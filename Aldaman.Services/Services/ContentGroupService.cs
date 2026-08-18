using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Aldaman.Persistence.Enums;
using Aldaman.Services.Configuration;
using Aldaman.Services.Constants;
using Aldaman.Services.Dtos.ContentGroup;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Helpers;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aldaman.Services.Services;

public sealed class ContentGroupService : IContentGroupService
{
    private AppDbContext Context { get; }
    private LocalizationSettings Localization { get; }

    public ContentGroupService(
        AppDbContext context,
        IOptions<LocalizationSettings> localizationOptions)
    {
        Context = context;
        Localization = localizationOptions.Value;
    }

    public async Task<PagedResultDto<ContentGroupListItemDto>> GetPagedContentGroupsAsync(PaginationQuery query, string? culture = null, bool filterDeleted = false, CancellationToken ct = default)
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

    public async Task<ContentGroupEditDto?> GetContentGroupForEditAsync(Guid id, CancellationToken ct = default)
    {
        var group = await Context.ContentGroups
            .IgnoreQueryFilters()
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (group == null) return null;

        return new ContentGroupEditDto
        {
            Id = group.Id,
            PlaceToShow = group.PlaceToShow,
            GroupOrder = group.GroupOrder,
            IsDeleted = group.IsDeleted,
            DeletedAtUtc = group.DeletedAtUtc,
            Translations = Localization.SupportedCultures.Select(culture =>
            {
                var translation = group.Translations.FirstOrDefault(t => t.CultureCode == culture);
                return new ContentGroupTranslationDto
                {
                    CultureCode = culture,
                    Title = translation?.Title ?? string.Empty,
                    Slug = translation?.Slug ?? string.Empty
                };
            }).ToList()
        };
    }

    public ContentGroupEditDto GetContentGroupForCreate()
    {
        return new ContentGroupEditDto
        {
            Translations = Localization.SupportedCultures.Select(c => new ContentGroupTranslationDto
            {
                CultureCode = c
            }).ToList()
        };
    }

    public async Task CreateContentGroupAsync(ContentGroupEditDto dto, CancellationToken ct = default)
    {
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
                CultureCode = translationDto.CultureCode,
                Title = translationDto.Title ?? string.Empty,
                Slug = !string.IsNullOrWhiteSpace(translationDto.Slug)
                    ? translationDto.Slug
                    : (translationDto.Title ?? string.Empty).ToLower().Replace(" ", "-")
            };

            group.Translations.Add(translation);
        }

        Context.ContentGroups.Add(group);
        await Context.SaveChangesAsync(ct);
    }

    public async Task UpdateContentGroupAsync(Guid id, ContentGroupEditDto dto, CancellationToken ct = default)
    {
        var group = await Context.ContentGroups
            .IgnoreQueryFilters()
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (group == null)
        {
            throw new KeyNotFoundException($"Content group with ID {id} not found.");
        }

        var currentTranslations = group.Translations.ToDictionary(t => t.CultureCode);

        // Update core group properties
        group.PlaceToShow = dto.PlaceToShow;
        group.GroupOrder = dto.GroupOrder;

        // Process translations
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

        await Context.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteContentGroupAsync(Guid id, CancellationToken ct = default)
    {
        var group = await Context.ContentGroups.FindAsync([id], cancellationToken: ct);
        if (group != null)
        {
            group.IsDeleted = true;
            group.DeletedAtUtc = DateTime.UtcNow;
            await Context.SaveChangesAsync(ct);
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
        }
    }

    public async Task HardDeleteContentGroupAsync(Guid id, CancellationToken ct = default)
    {
        var group = await Context.ContentGroups
            .IgnoreQueryFilters()
            .Include(p => p.Translations)
            .Include(p => p.ContentPages)
            .Include(p => p.BlogPosts)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (group != null)
        {
            if (group.Translations.Any())
            {
                Context.RemoveRange(group.Translations);
            }

            if (group.ContentPages.Any())
            {
                Context.RemoveRange(group.ContentPages);
            }

            if (group.BlogPosts.Any())
            {
                Context.RemoveRange(group.BlogPosts);
            }

            Context.ContentGroups.Remove(group);
            await Context.SaveChangesAsync(ct);
        }
    }
}
