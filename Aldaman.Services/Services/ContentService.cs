using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Aldaman.Persistence.Enums;
using Aldaman.Persistence.Interfaces;
using Aldaman.Services.Configuration;
using Aldaman.Services.Constants;
using Aldaman.Services.Dtos.Content;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Helpers;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Aldaman.Services.Services;

public sealed class ContentService : IContentService
{
    private static CancellationTokenSource _contentCacheTokenSource = new();

    private AppDbContext Context { get; }
    private LocalizationSettings Localization { get; }
    private IMediaService MediaService { get; }
    private IMemoryCache Cache { get; }
    private MemoryCacheEntryOptions CacheOptions { get; }
    private IUserContext UserContext { get; }

    public ContentService(
        AppDbContext context,
        IOptions<LocalizationSettings> localizationOptions,
        IOptions<CacheSettings> cacheOptions,
        IMediaService mediaService,
        IMemoryCache cache,
        IUserContext userContext)
    {
        Context = context;
        Localization = localizationOptions.Value;
        MediaService = mediaService;
        Cache = cache;
        CacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(cacheOptions.Value.ContentExpirationHours))
            .AddExpirationToken(new CancellationChangeToken(_contentCacheTokenSource.Token));
        UserContext = userContext;
    }

    /// <summary>
    /// Instantly invalidates all content-related cached entries, content groups, and navigation.
    /// It swaps the shared <see cref="_contentCacheTokenSource"/> with a new instance and cancels the old one,
    /// triggering eviction for all cache entries associated with the cancellation change token.
    /// </summary>
    internal static void InvalidateCache()
    {
        var oldSource = Interlocked.Exchange(ref _contentCacheTokenSource, new CancellationTokenSource());
        oldSource.Cancel();
        oldSource.Dispose();
        ContentGroupService.InvalidateCache();
        NavigationService.InvalidateCache();
    }

    #region Admin web part methods

    public async Task<PagedResultDto<ContentListItemDto>> GetPagedContentsAdminAsync(
        PaginationQuery query,
        string? culture = null,
        bool filterDeleted = false,
        CancellationToken ct = default)
    {
        var dbQuery = filterDeleted
            ? Context.Contents.IgnoreQueryFilters().Where(p => p.IsDeleted)
            : Context.Contents;

        dbQuery = dbQuery
            .Include(p => p.Translations)
            .Include(p => p.CoverMediaAsset)
            .AsQueryable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            dbQuery = dbQuery.Where(p => p.Translations.Any(
                t => t.Title.Contains(query.SearchTerm)
                || (t.Perex != null && t.Perex.Contains(query.SearchTerm))
                || (t.PlainText != null && t.PlainText.Contains(query.SearchTerm))));
        }

        // Sorting
        dbQuery = query.SortBy switch
        {
            SortByConstants.Title => query.SortDescending
                ? dbQuery.OrderByDescending(p => p.Translations.Where(t => culture == null || t.CultureCode == culture).Select(t => t.Title).FirstOrDefault())
                : dbQuery.OrderBy(p => p.Translations.Where(t => culture == null || t.CultureCode == culture).Select(t => t.Title).FirstOrDefault()),
            SortByConstants.CreatedAt => query.SortDescending
                ? dbQuery.OrderByDescending(p => p.CreatedAtUtc)
                : dbQuery.OrderBy(p => p.CreatedAtUtc),
            SortByConstants.PublishedAt => query.SortDescending
                ? dbQuery.OrderByDescending(p => p.PublishedAtUtc)
                : dbQuery.OrderBy(p => p.PublishedAtUtc),
            SortByConstants.PageOrder or nameof(ContentEntity.Order) => query.SortDescending
                ? dbQuery.OrderByDescending(p => p.Order)
                : dbQuery.OrderBy(p => p.Order),
            SortByConstants.DeletedAt => query.SortDescending
                ? dbQuery.OrderByDescending(p => p.DeletedAtUtc)
                : dbQuery.OrderBy(p => p.DeletedAtUtc),
            _ => filterDeleted
                ? dbQuery.OrderByDescending(p => p.DeletedAtUtc)
                : dbQuery.OrderByDescending(p => p.CreatedAtUtc)
        };

        var totalCount = await dbQuery.CountAsync(ct);
        var items = await dbQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ContentListItemDto
            {
                Id = p.Id,
                ContentType = p.ContentType,
                Title = p.Translations.FirstOrDefault(t => culture == null || t.CultureCode == culture)!.Title
                        ?? p.Translations.FirstOrDefault()!.Title
                        ?? string.Empty,
                Slug = p.Translations.FirstOrDefault(t => culture == null || t.CultureCode == culture)!.Slug
                       ?? p.Translations.FirstOrDefault()!.Slug
                       ?? string.Empty,
                Perex = p.Translations.FirstOrDefault(t => culture == null || t.CultureCode == culture)!.Perex,
                DisplayExpanded = p.Translations.Where(t => culture == null || t.CultureCode == culture).Select(t => t.DisplayExpanded).FirstOrDefault(),
                BodyHtml = p.Translations.Where(t => culture == null || t.CultureCode == culture).Select(t => t.BodyHtml).FirstOrDefault(),
                PlaceToShow = p.PlaceToShow,
                Order = p.Order,
                IsPublished = p.IsPublished,
                PublishedAtUtc = p.PublishedAtUtc,
                CoverImageRelativePath = p.CoverMediaAsset != null ? p.CoverMediaAsset.RelativePath : null,
                UpdatedAtUtc = p.UpdatedAtUtc == null
                    ? p.Translations.Max(t => t.UpdatedAtUtc)
                    : (p.Translations.Max(t => t.UpdatedAtUtc) > p.UpdatedAtUtc
                        ? p.Translations.Max(t => t.UpdatedAtUtc)
                        : p.UpdatedAtUtc),
                CreatedAtUtc = p.CreatedAtUtc,
                DeletedAtUtc = p.DeletedAtUtc
            })
            .ToListAsync(ct);

        return new PagedResultDto<ContentListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<ContentEditDto?> GetContentForEditAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await Context.Contents
            .IgnoreQueryFilters()
            .Include(p => p.Translations)
            .Include(p => p.CoverMediaAsset)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (entity == null) return null;

        return new ContentEditDto
        {
            Id = entity.Id,
            ContentType = entity.ContentType,
            PlaceToShow = entity.PlaceToShow,
            Order = entity.Order,
            IsDeleted = entity.IsDeleted,
            DeletedAtUtc = entity.DeletedAtUtc,
            CoverMediaAssetId = entity.CoverMediaAssetId,
            CoverImageRelativePath = entity.CoverMediaAsset?.RelativePath,
            IsPublished = entity.IsPublished,
            PublishedAtUtc = entity.PublishedAtUtc,
            Translations = Localization.SupportedCultures.Select(culture =>
            {
                var translation = entity.Translations.FirstOrDefault(t => t.CultureCode == culture);
                return new ContentTranslationDto
                {
                    CultureCode = culture,
                    Title = translation?.Title ?? string.Empty,
                    DisplayTitle = translation?.DisplayTitle ?? true,
                    Slug = translation?.Slug ?? string.Empty,
                    Perex = translation?.Perex,
                    BodyHtml = translation?.BodyHtml,
                    BodyDeltaJson = translation?.BodyDeltaJson,
                    PlainText = translation?.PlainText,
                    DisplayExpanded = translation?.DisplayExpanded ?? false
                };
            }).ToList()
        };
    }

    public ContentEditDto GetContentForCreate()
    {
        return new ContentEditDto
        {
            Translations = Localization.SupportedCultures.Select(c => new ContentTranslationDto
            {
                CultureCode = c,
                DisplayTitle = true
            }).ToList()
        };
    }

    public async Task CreateContentAsync(ContentEditDto dto, CancellationToken ct = default)
    {
        var entity = new ContentEntity
        {
            ContentType = dto.ContentType,
            PlaceToShow = dto.PlaceToShow,
            Order = dto.Order,
            CoverMediaAssetId = dto.CoverMediaAssetId,
            IsPublished = dto.IsPublished,
            PublishedAtUtc = dto.IsPublished ? (dto.PublishedAtUtc ?? DateTime.UtcNow) : null,
        };

        foreach (var translationDto in dto.Translations)
        {
            if (string.IsNullOrWhiteSpace(translationDto.Title) && string.IsNullOrWhiteSpace(translationDto.Slug))
                continue;

            var title = translationDto.Title?.Trim() ?? string.Empty;
            var slug = !string.IsNullOrWhiteSpace(translationDto.Slug)
                ? StringHelpers.ToSlug(translationDto.Slug)
                : StringHelpers.ToSlug(title);

            var translation = new ContentTranslationEntity
            {
                CultureCode = translationDto.CultureCode,
                Title = title,
                DisplayTitle = translationDto.DisplayTitle,
                Slug = slug,
                Perex = translationDto.Perex,
                BodyHtml = translationDto.BodyHtml,
                BodyDeltaJson = translationDto.BodyDeltaJson,
                PlainText = StringHelpers.StripHtml(translationDto.BodyHtml, ContentTranslationEntity.PlainTextMaxLength),
                DisplayExpanded = translationDto.DisplayExpanded
            };

            entity.Translations.Add(translation);
        }

        Context.Contents.Add(entity);
        await Context.SaveChangesAsync(ct);
        InvalidateCache();
    }

    public async Task UpdateContentAsync(Guid id, ContentEditDto dto, CancellationToken ct = default)
    {
        var entity = await Context.Contents
            .IgnoreQueryFilters()
            .Include(p => p.Translations)
            .Include(p => p.CoverMediaAsset)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (entity == null)
        {
            throw new KeyNotFoundException($"Content with ID {id} not found.");
        }

        // 1. Capture old state for media cleanup
        var oldMediaPaths = entity.Translations.ToDictionary(t => t.CultureCode, t => StringHelpers.ExtractMediaPaths(t.BodyHtml));
        var mediaToDelete = new HashSet<string>();

        // 2. Update core properties
        entity.ContentType = dto.ContentType;
        entity.PlaceToShow = dto.PlaceToShow;
        entity.Order = dto.Order;
        entity.IsPublished = dto.IsPublished;

        if (dto.RemoveCoverImage)
        {
            entity.CoverMediaAssetId = null;
        }
        else if (dto.CoverMediaAssetId.HasValue)
        {
            entity.CoverMediaAssetId = dto.CoverMediaAssetId;
        }

        if (dto.IsPublished && !entity.PublishedAtUtc.HasValue)
        {
            entity.PublishedAtUtc = dto.PublishedAtUtc ?? DateTime.UtcNow;
        }
        else if (dto.PublishedAtUtc.HasValue)
        {
            entity.PublishedAtUtc = dto.PublishedAtUtc;
        }

        // 3. Process translations following EF Core rules (DbSet Add/Remove, no direct collection mutation)
        var validCultureCodes = dto.Translations
            .Where(t => !string.IsNullOrWhiteSpace(t.Title) || !string.IsNullOrWhiteSpace(t.Slug) || !string.IsNullOrWhiteSpace(t.BodyHtml))
            .Select(t => t.CultureCode)
            .ToHashSet();

        var toRemove = entity.Translations.Where(t => !validCultureCodes.Contains(t.CultureCode)).ToList();
        if (toRemove.Count > 0)
        {
            foreach (var removedTrans in toRemove)
            {
                if (oldMediaPaths.TryGetValue(removedTrans.CultureCode, out var paths))
                {
                    foreach (var path in paths) mediaToDelete.Add(path);
                }
            }
            Context.ContentTranslations.RemoveRange(toRemove);
        }

        foreach (var translationDto in dto.Translations)
        {
            var title = translationDto.Title?.Trim() ?? string.Empty;
            var slug = translationDto.Slug?.Trim() ?? string.Empty;
            var bodyHtml = translationDto.BodyHtml;

            bool isEmpty = string.IsNullOrWhiteSpace(title) &&
                           string.IsNullOrWhiteSpace(slug) &&
                           string.IsNullOrWhiteSpace(bodyHtml);

            var existingTranslation = entity.Translations.FirstOrDefault(t => t.CultureCode == translationDto.CultureCode);

            if (isEmpty)
            {
                continue;
            }

            string finalSlug = !string.IsNullOrWhiteSpace(slug) ? StringHelpers.ToSlug(slug) : StringHelpers.ToSlug(title);
            string? plainText = StringHelpers.StripHtml(bodyHtml, ContentTranslationEntity.PlainTextMaxLength);

            if (existingTranslation != null)
            {
                // Check removed media in existing translation
                if (oldMediaPaths.TryGetValue(translationDto.CultureCode, out var oldPaths))
                {
                    var newPaths = StringHelpers.ExtractMediaPaths(bodyHtml).ToHashSet();
                    foreach (var oldPath in oldPaths)
                    {
                        if (!newPaths.Contains(oldPath))
                        {
                            mediaToDelete.Add(oldPath);
                        }
                    }
                }

                existingTranslation.Title = title;
                existingTranslation.DisplayTitle = translationDto.DisplayTitle;
                existingTranslation.Slug = finalSlug;
                existingTranslation.Perex = translationDto.Perex;
                existingTranslation.BodyHtml = bodyHtml;
                existingTranslation.BodyDeltaJson = translationDto.BodyDeltaJson;
                existingTranslation.PlainText = plainText;
                existingTranslation.DisplayExpanded = translationDto.DisplayExpanded;
            }
            else
            {
                var newTrans = new ContentTranslationEntity
                {
                    ContentId = entity.Id,
                    CultureCode = translationDto.CultureCode,
                    Title = title,
                    DisplayTitle = translationDto.DisplayTitle,
                    Slug = finalSlug,
                    Perex = translationDto.Perex,
                    BodyHtml = bodyHtml,
                    BodyDeltaJson = translationDto.BodyDeltaJson,
                    PlainText = plainText,
                    DisplayExpanded = translationDto.DisplayExpanded
                };
                Context.ContentTranslations.Add(newTrans);
            }
        }

        await Context.SaveChangesAsync(ct);
        InvalidateCache();

        // 4. Cleanup orphaned media files
        if (mediaToDelete.Any())
        {
            await MediaService.DeleteMediaAsync(mediaToDelete.ToList(), ct);
        }
    }

    public async Task SoftDeleteContentAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await Context.Contents.FindAsync([id], cancellationToken: ct);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            await Context.SaveChangesAsync(ct);
            InvalidateCache();
        }
    }

    public async Task RestoreContentAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await Context.Contents.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id, ct);
        if (entity != null)
        {
            entity.IsDeleted = false;
            entity.DeletedAtUtc = null;
            entity.DeletedByUserId = null;
            await Context.SaveChangesAsync(ct);
            InvalidateCache();
        }
    }

    public async Task HardDeleteContentAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await Context.Contents
            .IgnoreQueryFilters()
            .Include(p => p.Translations)
            .Include(p => p.ContentGroups)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (entity != null)
        {
            // Capture RTE media paths across translations
            var rteMediaPaths = entity.Translations
                .SelectMany(t => StringHelpers.ExtractMediaPaths(t.BodyHtml))
                .ToList();

            if (entity.Translations.Any())
            {
                Context.ContentTranslations.RemoveRange(entity.Translations);
            }

            if (entity.ContentGroups.Any())
            {
                Context.ContentGroupContents.RemoveRange(entity.ContentGroups);
            }

            Context.Contents.Remove(entity);
            await Context.SaveChangesAsync(ct);
            InvalidateCache();

            if (rteMediaPaths.Any())
            {
                await MediaService.DeleteMediaAsync(rteMediaPaths, ct);
            }
        }
    }

    #endregion

    #region Public web part methods

    public async Task<PagedResultDto<ContentListItemDto>> GetPagedContentsCachedAsync(
        int page,
        int pageSize,
        string culture,
        CancellationToken ct = default)
    {
        string cacheKey = $"Content:Paged:{page}:{pageSize}:{culture}";

        if (!Cache.TryGetValue(cacheKey, out PagedResultDto<ContentListItemDto>? result) || result == null)
        {
            var dbQuery = Context.Contents
                .Include(p => p.Translations)
                .Include(p => p.CoverMediaAsset)
                .Where(p => p.IsPublished && p.Translations.Any(t => t.CultureCode == culture))
                .OrderByDescending(p => p.PublishedAtUtc)
                .AsQueryable();

            var totalCount = await dbQuery.CountAsync(ct);
            var items = await dbQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ContentListItemDto
                {
                    Id = p.Id,
                    ContentType = p.ContentType,
                    Title = p.Translations.First(t => t.CultureCode == culture).Title,
                    Slug = p.Translations.First(t => t.CultureCode == culture).Slug,
                    Perex = p.Translations.First(t => t.CultureCode == culture).Perex,
                    DisplayExpanded = p.Translations.First(t => t.CultureCode == culture).DisplayExpanded,
                    BodyHtml = p.Translations.First(t => t.CultureCode == culture).DisplayExpanded
                        ? p.Translations.First(t => t.CultureCode == culture).BodyHtml
                        : null,
                    PublishedAtUtc = p.PublishedAtUtc,
                    IsPublished = p.IsPublished,
                    CoverImageRelativePath = p.CoverMediaAsset != null ? p.CoverMediaAsset.RelativePath : null,
                    PlaceToShow = p.PlaceToShow,
                    Order = p.Order,
                    CreatedAtUtc = p.CreatedAtUtc
                })
                .ToListAsync(ct);

            result = new PagedResultDto<ContentListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            Cache.Set(cacheKey, result, CacheOptions);
        }

        return result;
    }

    public async Task<ContentDetailDto?> GetContentBySlugCachedAsync(string slug, string culture, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(slug) || string.IsNullOrWhiteSpace(culture))
        {
            return null;
        }

        bool isAdmin = UserContext.IsAdminOrSuperAdmin;
        string cacheKey = $"Content:Slug:{culture}:{slug.ToLowerInvariant()}:{isAdmin}";

        if (!Cache.TryGetValue(cacheKey, out ContentDetailDto? result))
        {
            var query = isAdmin
                ? Context.Contents.IgnoreQueryFilters()
                : Context.Contents.Where(p => p.IsPublished);

            query = query
                .Include(p => p.Translations)
                .Include(p => p.CoverMediaAsset)
                .Include(p => p.CreatedByUser);

            var entity = await query
                .FirstOrDefaultAsync(p => p.Translations.Any(t => t.Slug == slug && t.CultureCode == culture), ct);

            if (entity == null)
            {
                result = null;
            }
            else
            {
                var translation = entity.Translations.FirstOrDefault(t => t.CultureCode == culture && t.Slug == slug)
                                  ?? entity.Translations.FirstOrDefault(t => t.CultureCode == culture)
                                  ?? entity.Translations.FirstOrDefault();

                if (translation == null)
                {
                    result = null;
                }
                else
                {
                    result = new ContentDetailDto
                    {
                        Id = entity.Id,
                        ContentType = entity.ContentType,
                        Title = translation.Title,
                        DisplayTitle = translation.DisplayTitle,
                        DisplayExpanded = translation.DisplayExpanded,
                        Slug = translation.Slug,
                        Perex = translation.Perex,
                        BodyHtml = translation.BodyHtml,
                        BodyDeltaJson = translation.BodyDeltaJson,
                        PlainText = translation.PlainText,
                        CoverImageRelativePath = entity.CoverMediaAsset?.RelativePath,
                        PublishedAtUtc = entity.PublishedAtUtc,
                        IsPublished = entity.IsPublished,
                        PlaceToShow = entity.PlaceToShow,
                        Order = entity.Order,
                        AuthorName = entity.CreatedByUser?.DisplayName
                    };
                }
            }

            Cache.Set(cacheKey, result, CacheOptions);
        }

        return result;
    }

    public async Task<IEnumerable<ContentDetailDto>> GetHomePageCachedAsync(string culture, CancellationToken ct = default)
    {
        string cacheKey = $"Content:Home:{culture}";

        if (!Cache.TryGetValue(cacheKey, out IEnumerable<ContentDetailDto>? result) || result == null)
        {
            result = await Context.Contents
                .Where(p => p.PlaceToShow.HasFlag(PlaceToShowEnum.HomePage))
                .Where(p => p.Translations.Any(t => t.CultureCode == culture))
                .Include(p => p.Translations)
                .Include(p => p.CoverMediaAsset)
                .OrderBy(p => p.Order)
                .Select(p => new ContentDetailDto
                {
                    Id = p.Id,
                    ContentType = p.ContentType,
                    Title = p.Translations.First(t => t.CultureCode == culture).Title,
                    DisplayTitle = p.Translations.First(t => t.CultureCode == culture).DisplayTitle,
                    DisplayExpanded = p.Translations.First(t => t.CultureCode == culture).DisplayExpanded,
                    Slug = p.Translations.First(t => t.CultureCode == culture).Slug,
                    Perex = p.Translations.First(t => t.CultureCode == culture).Perex,
                    BodyHtml = p.Translations.First(t => t.CultureCode == culture).BodyHtml,
                    BodyDeltaJson = p.Translations.First(t => t.CultureCode == culture).BodyDeltaJson,
                    PlainText = p.Translations.First(t => t.CultureCode == culture).PlainText,
                    CoverImageRelativePath = p.CoverMediaAsset != null ? p.CoverMediaAsset.RelativePath : null,
                    PublishedAtUtc = p.PublishedAtUtc,
                    IsPublished = p.IsPublished,
                    PlaceToShow = p.PlaceToShow,
                    Order = p.Order
                })
                .ToListAsync(ct);

            Cache.Set(cacheKey, result, CacheOptions);
        }

        return result;
    }

    public async Task<IEnumerable<ContentDetailDto>> GetSidebarBannersCachedAsync(string culture, CancellationToken ct = default)
    {
        string cacheKey = $"Content:Sidebar:{culture}";

        if (!Cache.TryGetValue(cacheKey, out IEnumerable<ContentDetailDto>? result) || result == null)
        {
            result = await Context.Contents
                .Where(p => p.PlaceToShow.HasFlag(PlaceToShowEnum.Sidebar) && p.IsPublished)
                .Where(p => p.Translations.Any(t => t.CultureCode == culture))
                .Include(p => p.Translations)
                .Include(p => p.CoverMediaAsset)
                .OrderBy(p => p.Order)
                .Select(p => new ContentDetailDto
                {
                    Id = p.Id,
                    ContentType = p.ContentType,
                    Title = p.Translations.First(t => t.CultureCode == culture).Title,
                    DisplayTitle = p.Translations.First(t => t.CultureCode == culture).DisplayTitle,
                    DisplayExpanded = p.Translations.First(t => t.CultureCode == culture).DisplayExpanded,
                    Slug = p.Translations.First(t => t.CultureCode == culture).Slug,
                    Perex = p.Translations.First(t => t.CultureCode == culture).Perex,
                    BodyHtml = p.Translations.First(t => t.CultureCode == culture).BodyHtml,
                    BodyDeltaJson = p.Translations.First(t => t.CultureCode == culture).BodyDeltaJson,
                    PlainText = p.Translations.First(t => t.CultureCode == culture).PlainText,
                    CoverImageRelativePath = p.CoverMediaAsset != null ? p.CoverMediaAsset.RelativePath : null,
                    PublishedAtUtc = p.PublishedAtUtc,
                    IsPublished = p.IsPublished,
                    PlaceToShow = p.PlaceToShow,
                    Order = p.Order
                })
                .ToListAsync(ct);

            Cache.Set(cacheKey, result, CacheOptions);
        }

        return result;
    }

    public async Task<Dictionary<string, string>> GetAlternativeSlugsCachedAsync(Guid id, CancellationToken ct = default)
    {
        string cacheKey = $"Content:AlternativeSlugs:{id}";

        if (!Cache.TryGetValue(cacheKey, out Dictionary<string, string>? result) || result == null)
        {
            result = await Context.ContentTranslations
                .Where(t => t.ContentId == id && !string.IsNullOrEmpty(t.Slug))
                .ToDictionaryAsync(t => t.CultureCode, t => t.Slug, ct);

            Cache.Set(cacheKey, result, CacheOptions);
        }

        return result;
    }

    public async Task<(ContentNavigationDto? Previous, ContentNavigationDto? Next)> GetContentNavigationCachedAsync(
        Guid currentContentId,
        string culture,
        CancellationToken ct = default)
    {
        string cacheKey = $"Content:Navigation:{currentContentId}:{culture}";

        if (!Cache.TryGetValue(cacheKey, out (ContentNavigationDto? Previous, ContentNavigationDto? Next) result))
        {
            var current = await Context.Contents.FindAsync([currentContentId], cancellationToken: ct);
            if (current == null || !current.PublishedAtUtc.HasValue)
            {
                result = (null, null);
            }
            else
            {
                var publishedAt = current.PublishedAtUtc.Value;

                // Previous content (older)
                var previous = await Context.Contents
                    .Include(p => p.Translations)
                    .Where(p => p.IsPublished && p.PublishedAtUtc < publishedAt && p.Translations.Any(t => t.CultureCode == culture))
                    .OrderByDescending(p => p.PublishedAtUtc)
                    .FirstOrDefaultAsync(ct);

                // Next content (newer)
                var next = await Context.Contents
                    .Include(p => p.Translations)
                    .Where(p => p.IsPublished && p.PublishedAtUtc > publishedAt && p.Translations.Any(t => t.CultureCode == culture))
                    .OrderBy(p => p.PublishedAtUtc)
                    .FirstOrDefaultAsync(ct);

                ContentNavigationDto? previousDto = null;
                if (previous != null)
                {
                    var translation = previous.Translations.First(t => t.CultureCode == culture);
                    previousDto = new ContentNavigationDto
                    {
                        Title = translation.Title,
                        Slug = translation.Slug
                    };
                }

                ContentNavigationDto? nextDto = null;
                if (next != null)
                {
                    var translation = next.Translations.First(t => t.CultureCode == culture);
                    nextDto = new ContentNavigationDto
                    {
                        Title = translation.Title,
                        Slug = translation.Slug
                    };
                }

                result = (previousDto, nextDto);
            }

            Cache.Set(cacheKey, result, CacheOptions);
        }

        return result;
    }

    public async Task<string?> GetRedirectSlugCachedAsync(string slug, string targetCulture, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(slug) || string.IsNullOrWhiteSpace(targetCulture))
        {
            return null;
        }

        string cacheKey = $"Content:RedirectSlug:{slug.ToLowerInvariant()}:{targetCulture}";

        if (!Cache.TryGetValue(cacheKey, out string? result))
        {
            var contentId = await Context.ContentTranslations
                .Where(t => t.Slug == slug && t.Content.IsPublished)
                .Select(t => (Guid?)t.ContentId)
                .FirstOrDefaultAsync(ct);

            if (contentId == null)
            {
                result = null;
            }
            else
            {
                result = await Context.ContentTranslations
                    .Where(t => t.ContentId == contentId.Value && t.CultureCode == targetCulture)
                    .Select(t => t.Slug)
                    .FirstOrDefaultAsync(ct);
            }

            Cache.Set(cacheKey, result, CacheOptions);
        }

        return result;
    }

    #endregion
}
