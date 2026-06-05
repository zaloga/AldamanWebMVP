using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Aldaman.Persistence.Enums;
using Aldaman.Services.Configuration;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Dtos.Page;
using Aldaman.Services.Helpers;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Aldaman.Services.Services;

public sealed class ContentPageService : IContentPageService
{
    private static CancellationTokenSource _pageCacheTokenSource = new();

    private AppDbContext Context { get; }
    private LocalizationSettings Localization { get; }
    private IMediaService MediaService { get; }
    private IMemoryCache Cache { get; }
    private MemoryCacheEntryOptions CacheOptions { get; }

    public ContentPageService(
        AppDbContext context,
        IOptions<LocalizationSettings> localizationOptions,
        IOptions<CacheSettings> cacheOptions,
        IMediaService mediaService,
        IMemoryCache cache)
    {
        Context = context;
        Localization = localizationOptions.Value;
        MediaService = mediaService;
        Cache = cache;
        CacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(cacheOptions.Value.ContentPageExpirationHours))
                .AddExpirationToken(new CancellationChangeToken(_pageCacheTokenSource.Token));
    }

    /// <summary>
    /// Instantly invalidates all blog-related cached entries.
    /// It swaps the shared <see cref="_blogCacheTokenSource"/> with a new instance and cancels the old one,
    /// triggering eviction for all cache entries associated with the cancellation change token.
    /// </summary>
    private static void InvalidateCache()
    {
        var oldSource = Interlocked.Exchange(ref _pageCacheTokenSource, new CancellationTokenSource());
        oldSource.Cancel();
        oldSource.Dispose();
    }

    #region Admin web part methods

    public async Task<PagedResultDto<ContentPageListItemDto>> GetPagedContentPagesAsync(PaginationQuery query, string? culture = null)
    {
        var dbQuery = Context.ContentPages
            .Include(p => p.Translations)
            .AsQueryable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            dbQuery = dbQuery.Where(p => p.PageKey.Contains(query.SearchTerm) || p.Translations.Any(c => c.Slug.Contains(query.SearchTerm)));
        }

        // Sorting
        dbQuery = query.SortBy switch
        {
            "Title" => query.SortDescending
                ? dbQuery.OrderByDescending(p => p.Translations.Where(t => culture == null || t.CultureCode == culture).Select(t => t.Title).FirstOrDefault())
                : dbQuery.OrderBy(p => p.Translations.Where(t => culture == null || t.CultureCode == culture).Select(t => t.Title).FirstOrDefault()),
            "PageKey" => query.SortDescending ? dbQuery.OrderByDescending(p => p.PageKey) : dbQuery.OrderBy(p => p.PageKey),
            "CreatedAt" => query.SortDescending ? dbQuery.OrderByDescending(p => p.CreatedAtUtc) : dbQuery.OrderBy(p => p.CreatedAtUtc),
            "PageOrder" => query.SortDescending ? dbQuery.OrderByDescending(p => p.PageOrder) : dbQuery.OrderBy(p => p.PageOrder),
            _ => dbQuery.OrderByDescending(p => p.CreatedAtUtc)
        };

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ContentPageListItemDto
            {
                Id = p.Id,
                PageKey = p.PageKey,
                Title = p.Translations.FirstOrDefault(t => culture == null || t.CultureCode == culture)!.Title
                        ?? p.Translations.FirstOrDefault()!.Title
                        ?? "-",
                PlaceToShow = p.PlaceToShow,
                PageOrder = p.PageOrder,
                CreatedAtUtc = p.CreatedAtUtc
            })
            .ToListAsync();

        return new PagedResultDto<ContentPageListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<ContentPageEditDto?> GetContentPageForEditAsync(Guid id)
    {
        var page = await Context.ContentPages
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (page == null) return null;

        return new ContentPageEditDto
        {
            Id = page.Id,
            PageKey = page.PageKey,
            PlaceToShow = page.PlaceToShow,
            PageOrder = page.PageOrder,
            Translations = Localization.SupportedCultures.Select(culture =>
            {
                var translation = page.Translations.FirstOrDefault(t => t.CultureCode == culture);
                return new ContentPageTranslationDto
                {
                    CultureCode = culture,
                    Title = translation?.Title ?? string.Empty,
                    Slug = translation?.Slug ?? string.Empty,
                    BodyHtml = translation?.BodyHtml,
                    BodyDeltaJson = translation?.BodyDeltaJson,
                    PlainText = translation?.PlainText
                };
            }).ToList()
        };
    }

    public ContentPageEditDto GetContentPageForCreate()
    {
        return new ContentPageEditDto
        {
            Translations = Localization.SupportedCultures.Select(c => new ContentPageTranslationDto
            {
                CultureCode = c
            }).ToList()
        };
    }

    public async Task CreateContentPageAsync(ContentPageEditDto dto)
    {
        var page = new ContentPageEntity
        {
            PageKey = dto.PageKey,
            PlaceToShow = dto.PlaceToShow,
            PageOrder = dto.PageOrder,
        };

        foreach (var translationDto in dto.Translations)
        {
            // Only add translation if it has some content or is a supported culture we want to ensure exists
            if (string.IsNullOrWhiteSpace(translationDto.Title) && string.IsNullOrWhiteSpace(translationDto.Slug))
                continue;

            var translation = new ContentPageTranslationEntity
            {
                CultureCode = translationDto.CultureCode,
                Title = translationDto.Title,
                Slug = !string.IsNullOrWhiteSpace(translationDto.Slug)
                    ? translationDto.Slug
                    : translationDto.Title.ToLower().Replace(" ", "-"),
                BodyHtml = translationDto.BodyHtml,
                BodyDeltaJson = translationDto.BodyDeltaJson,
                PlainText = StringHelpers.StripHtml(translationDto.BodyHtml, ContentPageTranslationEntity.PlainTextMaxLength)
            };

            page.Translations.Add(translation);
        }

        Context.ContentPages.Add(page);
        await Context.SaveChangesAsync();
        InvalidateCache();
    }

    public async Task UpdateContentPageAsync(Guid id, ContentPageEditDto dto)
    {
        var page = await Context.ContentPages
            .IgnoreQueryFilters()
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (page == null)
        {
            throw new KeyNotFoundException($"Page with ID {id} not found.");
        }

        // 1. Capture old state for media cleanup
        var oldMediaPaths = page.Translations.ToDictionary(t => t.CultureCode, t => StringHelpers.ExtractMediaPaths(t.BodyHtml));
        var currentTranslations = page.Translations.ToDictionary(t => t.CultureCode);
        var mediaToDelete = new HashSet<string>();

        // 2. Update core page properties
        page.PageKey = dto.PageKey;
        page.PlaceToShow = dto.PlaceToShow;
        page.PageOrder = dto.PageOrder;

        // 3. Process translations
        foreach (var translationDto in dto.Translations)
        {
            currentTranslations.TryGetValue(translationDto.CultureCode, out var existingTranslation);

            var title = translationDto.Title?.Trim() ?? string.Empty;
            var slug = translationDto.Slug?.Trim() ?? string.Empty;
            var bodyHtml = translationDto.BodyHtml;

            // Determine if the translation is effectively empty
            bool isEmpty = string.IsNullOrWhiteSpace(title) &&
                           string.IsNullOrWhiteSpace(slug) &&
                           string.IsNullOrWhiteSpace(bodyHtml);

            if (isEmpty)
            {
                if (existingTranslation != null)
                {
                    // Mark media for deletion from this removed translation
                    if (oldMediaPaths.TryGetValue(translationDto.CultureCode, out var paths))
                    {
                        foreach (var path in paths) mediaToDelete.Add(path);
                    }

                    Context.ContentPageTranslations.Remove(existingTranslation);
                }
                continue;
            }

            // If it doesn't exist in DB, create it
            if (existingTranslation == null)
            {
                existingTranslation = new ContentPageTranslationEntity
                {
                    CultureCode = translationDto.CultureCode,
                    ContentPageId = page.Id
                };
                Context.ContentPageTranslations.Add(existingTranslation);
            }
            else
            {
                // Calculate media diff for existing translation
                if (oldMediaPaths.TryGetValue(translationDto.CultureCode, out var oldPaths))
                {
                    var newPaths = StringHelpers.ExtractMediaPaths(bodyHtml);
                    foreach (var path in oldPaths.Except(newPaths))
                    {
                        mediaToDelete.Add(path);
                    }
                }
            }

            existingTranslation.Title = title;
            existingTranslation.Slug = !string.IsNullOrWhiteSpace(slug) ? slug : StringHelpers.ToSlug(title);
            existingTranslation.BodyHtml = bodyHtml;
            existingTranslation.BodyDeltaJson = translationDto.BodyDeltaJson;
            existingTranslation.PlainText = StringHelpers.StripHtml(bodyHtml, ContentPageTranslationEntity.PlainTextMaxLength);
        }

        await Context.SaveChangesAsync();
        InvalidateCache();

        // 4. Cleanup orphaned media files
        if (mediaToDelete.Any())
        {
            await MediaService.DeleteMediaAsync(mediaToDelete.ToList());
        }
    }

    public async Task SoftDeleteContentPageAsync(Guid id)
    {
        var page = await Context.ContentPages.FindAsync(id);
        if (page != null)
        {
            page.IsDeleted = true;
            page.DeletedAtUtc = DateTime.UtcNow;
            await Context.SaveChangesAsync();
            InvalidateCache();
        }
    }

    public async Task<PagedResultDto<ContentPageListItemDto>> GetPagedDeletedContentPagesAsync(PaginationQuery query, string? culture = null)
    {
        var dbQuery = Context.ContentPages
            .IgnoreQueryFilters()
            .Where(p => p.IsDeleted)
            .Include(p => p.Translations)
            .OrderByDescending(p => p.DeletedAtUtc)
            .AsQueryable();

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ContentPageListItemDto
            {
                Id = p.Id,
                PageKey = p.PageKey,
                Title = p.Translations.FirstOrDefault(t => culture == null || t.CultureCode == culture)!.Title
                        ?? p.Translations.FirstOrDefault()!.Title
                        ?? "-",
                PlaceToShow = p.PlaceToShow,
                PageOrder = p.PageOrder,
                CreatedAtUtc = p.CreatedAtUtc
            })
            .ToListAsync();

        return new PagedResultDto<ContentPageListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task RestoreContentPageAsync(Guid id)
    {
        var page = await Context.ContentPages.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id);
        if (page != null)
        {
            page.IsDeleted = false;
            page.DeletedAtUtc = null;
            page.DeletedByUserId = null;
            await Context.SaveChangesAsync();
            InvalidateCache();
        }
    }

    public async Task HardDeleteContentPageAsync(Guid id)
    {
        var page = await Context.ContentPages
            .IgnoreQueryFilters()
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (page != null)
        {
            // Explicitly remove translations
            if (page.Translations.Any())
            {
                Context.RemoveRange(page.Translations);
            }

            // Delete page
            Context.ContentPages.Remove(page);
            await Context.SaveChangesAsync();
            InvalidateCache();

            // Remove images used by RTE
            var rteMediaPaths = page.Translations
                .SelectMany(t => StringHelpers.ExtractMediaPaths(t.BodyHtml))
                .ToList();

            if (rteMediaPaths.Any())
            {
                await MediaService.DeleteMediaAsync(rteMediaPaths);
            }
        }
    }

    #endregion

    #region Public web part methods

    public async Task<PagedResultDto<ContentPageListItemDto>> GetPagedContentPagesCachedAsync(PaginationQuery query, string? culture = null)
    {
        string cacheKey = $"Page:Paged:{query.Page}:{query.PageSize}:{query.SearchTerm ?? ""}:{query.SortBy ?? ""}:{query.SortDescending}:{culture ?? ""}";

        if (!Cache.TryGetValue(cacheKey, out PagedResultDto<ContentPageListItemDto>? result) || result == null)
        {
            result = await GetPagedContentPagesAsync(query, culture);

            Cache.Set(cacheKey, result, CacheOptions);
        }

        return result;
    }

    public async Task<ContentPageDetailDto?> GetContentPageBySlugCachedAsync(string slug, string culture)
    {
        string cacheKey = $"Page:Slug:{culture}:{slug.ToLowerInvariant()}";

        if (!Cache.TryGetValue(cacheKey, out ContentPageDetailDto? result))
        {
            var page = await Context.ContentPages
                .Include(p => p.Translations)
                .FirstOrDefaultAsync(p => p.Translations.Any(c => c.Slug == slug && c.CultureCode == culture));

            if (page == null)
            {
                result = null;
            }
            else
            {
                var content = page.Translations.FirstOrDefault(t => t.CultureCode == culture);
                if (content == null)
                {
                    result = null;
                }
                else
                {
                    result = new ContentPageDetailDto
                    {
                        Id = page.Id,
                        PageKey = page.PageKey,
                        Title = content.Title,
                        Slug = content.Slug,
                        BodyHtml = content.BodyHtml,
                        BodyDeltaJson = content.BodyDeltaJson,
                        PlainText = content.PlainText
                    };
                }
            }

            Cache.Set(cacheKey, result, CacheOptions);
        }

        return result;
    }

    public async Task<IEnumerable<ContentPageDetailDto>> GetHomePageCachedAsync(string culture)
    {
        string cacheKey = $"Page:Home:{culture}";

        if (!Cache.TryGetValue(cacheKey, out IEnumerable<ContentPageDetailDto>? result) || result == null)
        {
            result = await Context.ContentPages
                .Where(p => p.PlaceToShow.HasFlag(PlaceToShowEnum.HomePage))
                .Where(p => p.Translations.Any(t => t.CultureCode == culture))
                .OrderBy(p => p.PageOrder)
                .Select(p => new ContentPageDetailDto
                {
                    Id = p.Id,
                    PageKey = p.PageKey,
                    Title = p.Translations.First(t => t.CultureCode == culture).Title,
                    Slug = p.Translations.First(t => t.CultureCode == culture).Slug,
                    BodyHtml = p.Translations.First(t => t.CultureCode == culture).BodyHtml,
                    BodyDeltaJson = p.Translations.First(t => t.CultureCode == culture).BodyDeltaJson,
                    PlainText = p.Translations.First(t => t.CultureCode == culture).PlainText
                })
                .ToListAsync();

            Cache.Set(cacheKey, result, CacheOptions);
        }

        return result;
    }

    public async Task<IEnumerable<ContentPageNavigationDto>> GetHomePageNavigationAsync(string culture)
    {
        return await GetNavigationInternalCachedAsync(culture, PlaceToShowEnum.HomePage);
    }

    public async Task<IEnumerable<ContentPageNavigationDto>> GetTopNavigationAsync(string culture)
    {
        return await GetNavigationInternalCachedAsync(culture, PlaceToShowEnum.TopNavigation);
    }

    public async Task<IEnumerable<ContentPageNavigationDto>> GetFooterNavigationAsync(string culture)
    {
        return await GetNavigationInternalCachedAsync(culture, PlaceToShowEnum.Footer);
    }

    private async Task<IEnumerable<ContentPageNavigationDto>> GetNavigationInternalCachedAsync(string culture, PlaceToShowEnum placeToShow)
    {
        string cacheKey = $"Page:Navigation:{culture}:{placeToShow}";

        if (!Cache.TryGetValue(cacheKey, out IEnumerable<ContentPageNavigationDto>? result) || result == null)
        {
            result = await Context.ContentPages
                .Where(p => p.PlaceToShow.HasFlag(placeToShow))
                .SelectMany(p => p.Translations.Where(t => t.CultureCode == culture))
                .Where(t => !string.IsNullOrEmpty(t.Title) && !string.IsNullOrEmpty(t.Slug))
                .OrderBy(t => t.ContentPage.PageOrder)
                .Select(t => new ContentPageNavigationDto
                {
                    PageKey = t.ContentPage.PageKey,
                    Title = t.Title,
                    Slug = t.Slug
                })
                .ToListAsync();

            Cache.Set(cacheKey, result, CacheOptions);
        }

        return result;
    }

    public async Task<Dictionary<string, string>> GetAlternativeSlugsCachedAsync(Guid id)
    {
        string cacheKey = $"Page:AlternativeSlugs:{id}";

        if (!Cache.TryGetValue(cacheKey, out Dictionary<string, string>? result) || result == null)
        {
            result = await Context.ContentPageTranslations
                .Where(t => t.ContentPageId == id)
                .ToDictionaryAsync(t => t.CultureCode, t => t.Slug);

            Cache.Set(cacheKey, result, CacheOptions);
        }

        return result;
    }

    public async Task<string?> GetRedirectSlugCachedAsync(string slug, string targetCulture)
    {
        string cacheKey = $"Page:RedirectSlug:{slug.ToLowerInvariant()}:{targetCulture}";

        if (!Cache.TryGetValue(cacheKey, out string? result))
        {
            var pageId = await Context.ContentPageTranslations
                .Where(t => t.Slug == slug)
                .Select(t => (Guid?)t.ContentPageId)
                .FirstOrDefaultAsync();

            if (pageId == null)
            {
                result = null;
            }
            else
            {
                result = await Context.ContentPageTranslations
                    .Where(t => t.ContentPageId == pageId.Value && t.CultureCode == targetCulture)
                    .Select(t => t.Slug)
                    .FirstOrDefaultAsync();
            }

            Cache.Set(cacheKey, result, CacheOptions);
        }

        return result;
    }

    #endregion
}
