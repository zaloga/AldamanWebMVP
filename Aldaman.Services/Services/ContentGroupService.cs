using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Aldaman.Persistence.Enums;
using Aldaman.Services.Configuration;
using Aldaman.Services.Constants;
using Aldaman.Services.Dtos.Content;
using Aldaman.Services.Dtos.ContentGroup;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Helpers;
using Aldaman.Services.Interfaces;
using Aldaman.Services.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Aldaman.Services.Services;

public sealed class ContentGroupService : IContentGroupService
{
    private static CancellationTokenSource _contentGroupCacheTokenSource = new();

    private AppDbContext Context { get; }
    private LocalizationSettings Localization { get; }
    private IMemoryCache Cache { get; }
    private MemoryCacheEntryOptions CacheOptions { get; }
    private IStringLocalizer<ValidationResources> Localizer { get; }

    public ContentGroupService(
        AppDbContext context,
        IOptions<LocalizationSettings> localizationOptions,
        IOptions<CacheSettings> cacheOptions,
        IMemoryCache cache,
        IStringLocalizer<ValidationResources> localizer)
    {
        Context = context;
        Localization = localizationOptions.Value;
        Cache = cache;
        Localizer = localizer;
        CacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(cacheOptions.Value.ContentExpirationHours))
            .AddExpirationToken(new CancellationChangeToken(_contentGroupCacheTokenSource.Token));
    }

    /// <summary>
    /// Instantly invalidates all content group related cached entries and navigation caches.
    /// </summary>
    internal static void InvalidateCache()
    {
        var oldSource = Interlocked.Exchange(ref _contentGroupCacheTokenSource, new CancellationTokenSource());
        oldSource.Cancel();
        oldSource.Dispose();
        NavigationService.InvalidateCache();
    }

    public async Task<PagedResultDto<ContentGroupListItemDto>> GetPagedContentGroupsAsync(
        PaginationQuery query,
        string? culture = null,
        bool filterDeleted = false,
        CancellationToken ct = default)
    {
        var dbQuery = filterDeleted
            ? Context.ContentGroups.IgnoreQueryFilters().Where(p => p.IsDeleted)
            : Context.ContentGroups;

        dbQuery = dbQuery
            .Include(p => p.Translations)
            .AsQueryable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            dbQuery = dbQuery.Where(p => p.Translations.Any(c => c.Slug.Contains(query.SearchTerm) || c.Title.Contains(query.SearchTerm)));
        }

        // Sorting
        dbQuery = query.SortBy switch
        {
            SortByConstants.Title => query.SortDescending
                ? dbQuery.OrderByDescending(p => p.Translations.Where(t => culture == null || t.CultureCode == culture).Select(t => t.Title).FirstOrDefault())
                : dbQuery.OrderBy(p => p.Translations.Where(t => culture == null || t.CultureCode == culture).Select(t => t.Title).FirstOrDefault()),
            SortByConstants.CreatedAt => query.SortDescending ? dbQuery.OrderByDescending(p => p.CreatedAtUtc) : dbQuery.OrderBy(p => p.CreatedAtUtc),
            SortByConstants.GroupOrder or SortByConstants.PageOrder => query.SortDescending ? dbQuery.OrderByDescending(p => p.GroupOrder) : dbQuery.OrderBy(p => p.GroupOrder),
            SortByConstants.DeletedAt => query.SortDescending ? dbQuery.OrderByDescending(p => p.DeletedAtUtc) : dbQuery.OrderBy(p => p.DeletedAtUtc),
            _ => filterDeleted
                ? dbQuery.OrderByDescending(p => p.DeletedAtUtc)
                : dbQuery.OrderByDescending(p => p.CreatedAtUtc)
        };

        var totalCount = await dbQuery.CountAsync(ct);
        var items = await dbQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ContentGroupListItemDto
            {
                Id = p.Id,
                Title = p.Translations.FirstOrDefault(t => culture == null || t.CultureCode == culture)!.Title
                        ?? p.Translations.FirstOrDefault()!.Title
                        ?? string.Empty,
                Slug = p.Translations.FirstOrDefault(t => culture == null || t.CultureCode == culture)!.Slug
                        ?? p.Translations.FirstOrDefault()!.Slug
                        ?? string.Empty,
                PlaceToShow = p.PlaceToShow,
                GroupOrder = p.GroupOrder,
                UpdatedAtUtc = p.UpdatedAtUtc == null
                    ? p.Translations.Max(t => t.UpdatedAtUtc)
                    : (p.Translations.Max(t => (DateTime?)t.UpdatedAtUtc) > p.UpdatedAtUtc
                        ? p.Translations.Max(t => t.UpdatedAtUtc)
                        : p.UpdatedAtUtc),
                CreatedAtUtc = p.CreatedAtUtc,
                DeletedAtUtc = p.DeletedAtUtc
            })
            .ToListAsync(ct);

        return new PagedResultDto<ContentGroupListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<ContentGroupEditDto?> GetContentGroupForEditAsync(Guid id, string? culture = null, CancellationToken ct = default)
    {
        var group = await Context.ContentGroups
            .IgnoreQueryFilters()
            .Include(p => p.Translations)
            .Include(p => p.Contents.OrderBy(c => c.Order))
                .ThenInclude(c => c.Content)
                    .ThenInclude(c => c.Translations)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (group == null) return null;

        var dto = new ContentGroupEditDto
        {
            Id = group.Id,
            PlaceToShow = group.PlaceToShow,
            GroupOrder = group.GroupOrder,
            IsDeleted = group.IsDeleted,
            DeletedAtUtc = group.DeletedAtUtc,
            Translations = Localization.SupportedCultures.Select(c =>
            {
                var translation = group.Translations.FirstOrDefault(t => t.CultureCode == c);
                return new ContentGroupTranslationDto
                {
                    CultureCode = c,
                    Title = translation?.Title ?? string.Empty,
                    Slug = translation?.Slug ?? string.Empty
                };
            }).ToList(),
            SelectedItems = group.Contents.Select(c => new ContentGroupItemSelectionDto
            {
                Id = c.ContentId,
                Title = c.Content.Translations.FirstOrDefault(t => culture == null || t.CultureCode == culture)?.Title
                        ?? c.Content.Translations.FirstOrDefault()?.Title
                        ?? string.Empty,
                Slug = c.Content.Translations.FirstOrDefault(t => culture == null || t.CultureCode == culture)?.Slug
                        ?? c.Content.Translations.FirstOrDefault()?.Slug
                        ?? string.Empty,
                Order = c.Order
            })
            .OrderBy(x => x.Order)
            .ToList()
        };

        await PopulateAvailableOptionsAsync(dto, culture, ct);

        return dto;
    }

    public async Task<ContentGroupEditDto> GetContentGroupForCreateAsync(string? culture = null, CancellationToken ct = default)
    {
        var dto = new ContentGroupEditDto
        {
            Translations = Localization.SupportedCultures.Select(c => new ContentGroupTranslationDto
            {
                CultureCode = c
            }).ToList()
        };

        await PopulateAvailableOptionsAsync(dto, culture, ct);

        return dto;
    }

    public async Task PopulateAvailableOptionsAsync(ContentGroupEditDto dto, string? culture = null, CancellationToken ct = default)
    {
        dto.AvailableContents = await Context.Contents
            .Include(p => p.Translations)
            .OrderBy(p => p.Order)
            .Select(p => new ContentGroupItemOptionDto
            {
                Id = p.Id,
                Title = p.Translations.FirstOrDefault(t => culture == null || t.CultureCode == culture)!.Title
                        ?? p.Translations.FirstOrDefault()!.Title
                        ?? string.Empty,
                Slug = p.Translations.FirstOrDefault(t => culture == null || t.CultureCode == culture)!.Slug
                        ?? p.Translations.FirstOrDefault()!.Slug
                        ?? string.Empty
            })
            .ToListAsync(ct);
    }

    public async Task CreateContentGroupAsync(ContentGroupEditDto dto, CancellationToken ct = default)
    {
        if (dto.PlaceToShow.HasFlag(PlaceToShowEnum.HomePage))
        {
            bool hasOtherHomePageGroup = await Context.ContentGroups
                .AnyAsync(g => g.PlaceToShow.HasFlag(PlaceToShowEnum.HomePage), ct);

            if (hasOtherHomePageGroup)
            {
                throw new InvalidOperationException(Localizer[ValidationResources.OnlyOneContentGroupAllowedOnHomePage].Value);
            }
        }

        var group = new ContentGroupEntity
        {
            PlaceToShow = dto.PlaceToShow,
            GroupOrder = dto.GroupOrder
        };

        foreach (var translationDto in dto.Translations)
        {
            if (string.IsNullOrWhiteSpace(translationDto.Title) && string.IsNullOrWhiteSpace(translationDto.Slug))
                continue;

            var translation = new ContentGroupTranslationEntity
            {
                ContentGroupId = group.Id,
                CultureCode = translationDto.CultureCode,
                Title = translationDto.Title ?? string.Empty,
                Slug = !string.IsNullOrWhiteSpace(translationDto.Slug)
                    ? translationDto.Slug
                    : (translationDto.Title ?? string.Empty).ToLower().Replace(" ", "-")
            };

            group.Translations.Add(translation);
        }

        if (dto.SelectedItems != null)
        {
            int itemIndex = 0;
            foreach (var item in dto.SelectedItems.Where(x => x.Id != Guid.Empty))
            {
                int resolvedOrder = item.Order != 0 ? item.Order : itemIndex;
                group.Contents.Add(new ContentGroupContentEntity
                {
                    ContentGroupId = group.Id,
                    ContentId = item.Id,
                    Order = resolvedOrder
                });
                itemIndex++;
            }
        }

        Context.ContentGroups.Add(group);
        await Context.SaveChangesAsync(ct);
        InvalidateCache();
    }

    public async Task UpdateContentGroupAsync(Guid id, ContentGroupEditDto dto, CancellationToken ct = default)
    {
        var group = await Context.ContentGroups
            .IgnoreQueryFilters()
            .Include(p => p.Translations)
            .Include(p => p.Contents)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (group == null)
        {
            throw new KeyNotFoundException($"Content group with ID {id} not found.");
        }

        if (dto.PlaceToShow.HasFlag(PlaceToShowEnum.HomePage))
        {
            bool hasOtherHomePageGroup = await Context.ContentGroups
                .AnyAsync(g => g.Id != id && g.PlaceToShow.HasFlag(PlaceToShowEnum.HomePage), ct);

            if (hasOtherHomePageGroup)
            {
                throw new InvalidOperationException(Localizer[ValidationResources.OnlyOneContentGroupAllowedOnHomePage].Value);
            }
        }

        group.PlaceToShow = dto.PlaceToShow;
        group.GroupOrder = dto.GroupOrder;

        // Process Translations
        var currentTranslations = group.Translations.ToDictionary(t => t.CultureCode);

        foreach (var translationDto in dto.Translations)
        {
            currentTranslations.TryGetValue(translationDto.CultureCode, out var existingTranslation);

            var title = translationDto.Title?.Trim() ?? string.Empty;
            var slug = translationDto.Slug?.Trim() ?? string.Empty;

            bool isEmpty = string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(slug);

            if (isEmpty)
            {
                if (existingTranslation != null)
                {
                    Context.ContentGroupTranslations.Remove(existingTranslation);
                }
                continue;
            }

            if (existingTranslation == null)
            {
                existingTranslation = new ContentGroupTranslationEntity
                {
                    CultureCode = translationDto.CultureCode,
                    ContentGroupId = group.Id
                };
                Context.ContentGroupTranslations.Add(existingTranslation);
            }

            existingTranslation.Title = title;
            existingTranslation.Slug = !string.IsNullOrWhiteSpace(slug) ? slug : StringHelpers.ToSlug(title);
        }

        // Process unified SelectedItems
        var selectedItems = (dto.SelectedItems ?? new List<ContentGroupItemSelectionDto>())
            .Where(x => x.Id != Guid.Empty)
            .ToList();

        var selectedIds = selectedItems.Select(x => x.Id).ToHashSet();

        // 1. Remove deleted items via DbSet
        var toRemove = group.Contents.Where(c => !selectedIds.Contains(c.ContentId)).ToList();
        if (toRemove.Count > 0)
        {
            Context.ContentGroupContents.RemoveRange(toRemove);
        }

        // 2. Add or update items
        foreach (var item in selectedItems)
        {
            var existing = group.Contents.FirstOrDefault(c => c.ContentId == item.Id);
            if (existing != null)
            {
                existing.Order = item.Order;
            }
            else
            {
                var newEntity = new ContentGroupContentEntity
                {
                    ContentGroupId = group.Id,
                    ContentId = item.Id,
                    Order = item.Order
                };
                Context.ContentGroupContents.Add(newEntity);
            }
        }

        await Context.SaveChangesAsync(ct);
        InvalidateCache();
    }

    public async Task SoftDeleteContentGroupAsync(Guid id, CancellationToken ct = default)
    {
        var group = await Context.ContentGroups.FindAsync([id], cancellationToken: ct);
        if (group != null)
        {
            group.IsDeleted = true;
            group.DeletedAtUtc = DateTime.UtcNow;
            await Context.SaveChangesAsync(ct);
            InvalidateCache();
        }
    }

    public async Task RestoreContentGroupAsync(Guid id, CancellationToken ct = default)
    {
        var group = await Context.ContentGroups.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id, ct);
        if (group != null)
        {
            group.IsDeleted = false;
            group.DeletedAtUtc = null;
            group.DeletedByUserId = null;
            await Context.SaveChangesAsync(ct);
            InvalidateCache();
        }
    }

    public async Task HardDeleteContentGroupAsync(Guid id, CancellationToken ct = default)
    {
        var group = await Context.ContentGroups
            .IgnoreQueryFilters()
            .Include(p => p.Translations)
            .Include(p => p.Contents)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (group != null)
        {
            if (group.Translations.Any())
            {
                Context.ContentGroupTranslations.RemoveRange(group.Translations);
            }

            if (group.Contents.Any())
            {
                Context.ContentGroupContents.RemoveRange(group.Contents);
            }

            Context.ContentGroups.Remove(group);
            await Context.SaveChangesAsync(ct);
            InvalidateCache();
        }
    }

    #region Public web part methods

    public async Task<ContentGroupDetailDto?> GetContentGroupBySlugCachedAsync(string slug, string culture, CancellationToken ct = default)
    {
        string cacheKey = $"ContentGroup:Detail:{slug.ToLowerInvariant()}:{culture}";

        if (!Cache.TryGetValue(cacheKey, out ContentGroupDetailDto? result))
        {
            var group = await Context.ContentGroups
                .Include(p => p.Translations)
                .Include(p => p.Contents)
                    .ThenInclude(c => c.Content)
                        .ThenInclude(c => c.Translations)
                .Include(p => p.Contents)
                    .ThenInclude(c => c.Content)
                        .ThenInclude(c => c.CoverMediaAsset)
                .FirstOrDefaultAsync(p => p.Translations.Any(t => t.Slug == slug && t.CultureCode == culture), ct);

            if (group == null)
            {
                result = null;
            }
            else
            {
                var translation = group.Translations.FirstOrDefault(t => t.CultureCode == culture);
                if (translation == null)
                {
                    result = null;
                }
                else
                {
                    var items = new List<ContentGroupDetailItemDto>();

                    foreach (var c in group.Contents.Where(c => !c.Content.IsDeleted && c.Content.IsPublished))
                    {
                        var contentTranslation = c.Content.Translations.FirstOrDefault(t => t.CultureCode == culture)
                                                 ?? c.Content.Translations.FirstOrDefault();
                        if (contentTranslation != null)
                        {
                            items.Add(new ContentGroupDetailItemDto
                            {
                                Id = c.ContentId,
                                Order = c.Order,
                                Content = new ContentDetailDto
                                {
                                    Id = c.Content.Id,
                                    ContentType = c.Content.ContentType,
                                    Title = contentTranslation.Title,
                                    DisplayTitle = contentTranslation.DisplayTitle,
                                    DisplayExpanded = contentTranslation.DisplayExpanded,
                                    Slug = contentTranslation.Slug,
                                    Perex = contentTranslation.Perex,
                                    BodyHtml = contentTranslation.BodyHtml,
                                    BodyDeltaJson = contentTranslation.BodyDeltaJson,
                                    PlainText = contentTranslation.PlainText,
                                    CoverImageRelativePath = c.Content.CoverMediaAsset?.RelativePath,
                                    PublishedAtUtc = c.Content.PublishedAtUtc,
                                    IsPublished = c.Content.IsPublished,
                                    PlaceToShow = c.Content.PlaceToShow,
                                    Order = c.Content.Order
                                }
                            });
                        }
                    }

                    result = new ContentGroupDetailDto
                    {
                        Id = group.Id,
                        Title = translation.Title,
                        Slug = translation.Slug,
                        PlaceToShow = group.PlaceToShow,
                        GroupOrder = group.GroupOrder,
                        Items = items.OrderBy(x => x.Order).ToList()
                    };
                }
            }

            Cache.Set(cacheKey, result, CacheOptions);
        }

        return result;
    }

    public async Task<Dictionary<string, string>> GetAlternativeSlugsCachedAsync(Guid id, CancellationToken ct = default)
    {
        string cacheKey = $"ContentGroup:AlternativeSlugs:{id}";

        if (!Cache.TryGetValue(cacheKey, out Dictionary<string, string>? result) || result == null)
        {
            result = await Context.ContentGroupTranslations
                .Where(t => t.ContentGroupId == id)
                .ToDictionaryAsync(t => t.CultureCode, t => t.Slug, ct);

            Cache.Set(cacheKey, result, CacheOptions);
        }

        return result;
    }

    public async Task<ContentGroupDetailDto?> GetHomePageContentGroupCachedAsync(string culture, CancellationToken ct = default)
    {
        string cacheKey = $"ContentGroup:HomePage:{culture}";

        if (!Cache.TryGetValue(cacheKey, out ContentGroupDetailDto? result))
        {
            var group = await Context.ContentGroups
                .Include(p => p.Translations)
                .Include(p => p.Contents)
                    .ThenInclude(c => c.Content)
                        .ThenInclude(c => c.Translations)
                .Include(p => p.Contents)
                    .ThenInclude(c => c.Content)
                        .ThenInclude(c => c.CoverMediaAsset)
                .FirstOrDefaultAsync(p => p.PlaceToShow.HasFlag(PlaceToShowEnum.HomePage), ct);

            if (group == null)
            {
                result = null;
            }
            else
            {
                var translation = group.Translations.FirstOrDefault(t => t.CultureCode == culture)
                                  ?? group.Translations.FirstOrDefault(t => t.CultureCode == Localization.DefaultCulture)
                                  ?? group.Translations.FirstOrDefault();

                if (translation == null)
                {
                    result = null;
                }
                else
                {
                    var items = new List<ContentGroupDetailItemDto>();

                    foreach (var c in group.Contents.Where(c => !c.Content.IsDeleted && c.Content.IsPublished))
                    {
                        var contentTranslation = c.Content.Translations.FirstOrDefault(t => t.CultureCode == culture)
                                                 ?? c.Content.Translations.FirstOrDefault(t => t.CultureCode == Localization.DefaultCulture)
                                                 ?? c.Content.Translations.FirstOrDefault();

                        if (contentTranslation != null)
                        {
                            items.Add(new ContentGroupDetailItemDto
                            {
                                Id = c.ContentId,
                                Order = c.Order,
                                Content = new ContentDetailDto
                                {
                                    Id = c.Content.Id,
                                    ContentType = c.Content.ContentType,
                                    Title = contentTranslation.Title,
                                    DisplayTitle = contentTranslation.DisplayTitle,
                                    DisplayExpanded = contentTranslation.DisplayExpanded,
                                    Slug = contentTranslation.Slug,
                                    Perex = contentTranslation.Perex,
                                    BodyHtml = contentTranslation.BodyHtml,
                                    BodyDeltaJson = contentTranslation.BodyDeltaJson,
                                    PlainText = contentTranslation.PlainText,
                                    CoverImageRelativePath = c.Content.CoverMediaAsset?.RelativePath,
                                    PublishedAtUtc = c.Content.PublishedAtUtc,
                                    IsPublished = c.Content.IsPublished,
                                    PlaceToShow = c.Content.PlaceToShow,
                                    Order = c.Content.Order
                                }
                            });
                        }
                    }

                    result = new ContentGroupDetailDto
                    {
                        Id = group.Id,
                        Title = translation.Title,
                        Slug = translation.Slug,
                        PlaceToShow = group.PlaceToShow,
                        GroupOrder = group.GroupOrder,
                        Items = items.OrderBy(x => x.Order).ToList()
                    };
                }
            }

            Cache.Set(cacheKey, result, CacheOptions);
        }

        return result;
    }

    public async Task<string?> GetRedirectSlugCachedAsync(string slug, string targetCulture, CancellationToken ct = default)
    {
        string cacheKey = $"ContentGroup:RedirectSlug:{slug.ToLowerInvariant()}:{targetCulture}";

        if (!Cache.TryGetValue(cacheKey, out string? result))
        {
            var groupId = await Context.ContentGroupTranslations
                .Where(t => t.Slug == slug)
                .Select(t => (Guid?)t.ContentGroupId)
                .FirstOrDefaultAsync(ct);

            if (groupId == null)
            {
                result = null;
            }
            else
            {
                result = await Context.ContentGroupTranslations
                    .Where(t => t.ContentGroupId == groupId.Value && t.CultureCode == targetCulture)
                    .Select(t => t.Slug)
                    .FirstOrDefaultAsync(ct);
            }

            Cache.Set(cacheKey, result, CacheOptions);
        }

        return result;
    }

    #endregion
}
