using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Aldaman.Services.Configuration;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Dtos.Page;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aldaman.Services.Services;

public sealed class ContentPageService : IContentPageService
{
    private AppDbContext Context { get; }
    private LocalizationSettings Localization { get; }

    public ContentPageService(AppDbContext context, IOptions<LocalizationSettings> localizationOptions)
    {
        Context = context;
        Localization = localizationOptions.Value;
    }

    public async Task<PagedResultDto<ContentPageListItemDto>> GetPagedPagesAsync(PaginationQuery query)
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
            "PageKey" => query.SortDescending ? dbQuery.OrderByDescending(p => p.PageKey) : dbQuery.OrderBy(p => p.PageKey),
            "CreatedAt" => query.SortDescending ? dbQuery.OrderByDescending(p => p.CreatedAtUtc) : dbQuery.OrderBy(p => p.CreatedAtUtc),
            "SortOrderInNavigation" => query.SortDescending ? dbQuery.OrderByDescending(p => p.OrderInNavigation) : dbQuery.OrderBy(p => p.OrderInNavigation),
            "SortOrderOnHomePage" => query.SortDescending ? dbQuery.OrderByDescending(p => p.OrderOnHomePage) : dbQuery.OrderBy(p => p.OrderOnHomePage),
            _ => dbQuery.OrderBy(p => p.OrderInNavigation)
        };

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ContentPageListItemDto
            {
                Id = p.Id,
                PageKey = p.PageKey,
                ShowOnHomePage = p.ShowOnHomePage,
                OrderOnHomePage = p.OrderOnHomePage,
                OrderInNavigation = p.OrderInNavigation,
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

    public async Task<ContentPageDetailDto?> GetPageBySlugAsync(string slug, string culture)
    {
        var page = await Context.ContentPages
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Translations.Any(c => c.Slug == slug && c.CultureCode == culture));

        if (page == null) return null;

        var content = page.Translations.FirstOrDefault(t => t.CultureCode == culture);
        if (content == null) return null;

        return new ContentPageDetailDto
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

    public async Task<IEnumerable<ContentPageDetailDto>> GetHomePageAsync(string culture)
    {
        var pages = await Context.ContentPages
            .Include(p => p.Translations)
            .Where(p => p.ShowOnHomePage)
            .OrderBy(p => p.OrderOnHomePage)
            .ToListAsync();

        return pages.Select(page =>
        {
            var content = page.Translations.FirstOrDefault(t => t.CultureCode == culture)
                         ?? page.Translations.FirstOrDefault();

            return new ContentPageDetailDto
            {
                Id = page.Id,
                PageKey = page.PageKey,
                Title = content?.Title ?? string.Empty,
                Slug = content?.Slug ?? string.Empty,
                BodyHtml = content?.BodyHtml,
                BodyDeltaJson = content?.BodyDeltaJson,
                PlainText = content?.PlainText
            };
        }).ToList();
    }

    public async Task<IEnumerable<ContentPageNavigationDto>> GetNavigationPagesAsync(string culture)
    {
        var pages = await Context.ContentPages
            .Include(p => p.Translations)
            .Where(p => !p.ShowOnHomePage) // Assuming we don't want to show home page items in navigation
            .OrderBy(p => p.OrderInNavigation)
            .ToListAsync();

        return pages.Select(page =>
        {
            var content = page.Translations.FirstOrDefault(t => t.CultureCode == culture)
                         ?? page.Translations.FirstOrDefault();

            return new ContentPageNavigationDto
            {
                Title = content?.Title ?? string.Empty,
                Slug = content?.Slug ?? string.Empty
            };
        }).ToList();
    }

    public async Task<ContentPageEditDto?> GetPageForEditAsync(Guid id)
    {
        var page = await Context.ContentPages
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (page == null) return null;

        return new ContentPageEditDto
        {
            Id = page.Id,
            PageKey = page.PageKey,
            ShowOnHomePage = page.ShowOnHomePage,
            OrderOnHomePage = page.OrderOnHomePage,
            OrderInNavigation = page.OrderInNavigation,
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

    public ContentPageEditDto GetPageForCreate()
    {
        return new ContentPageEditDto
        {
            Translations = Localization.SupportedCultures.Select(c => new ContentPageTranslationDto
            {
                CultureCode = c
            }).ToList()
        };
    }

    public async Task CreatePageAsync(ContentPageEditDto dto)
    {
        var page = new ContentPageEntity
        {
            PageKey = dto.PageKey,
            ShowOnHomePage = dto.ShowOnHomePage,
            OrderOnHomePage = dto.OrderOnHomePage,
            OrderInNavigation = dto.OrderInNavigation
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
                PlainText = StripHtml(translationDto.BodyHtml)
            };

            page.Translations.Add(translation);
        }

        Context.ContentPages.Add(page);
        await Context.SaveChangesAsync();
    }

    public async Task UpdatePageAsync(Guid id, ContentPageEditDto dto)
    {
        var page = await Context.ContentPages
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (page == null)
        {
            throw new KeyNotFoundException($"Page with ID {id} not found.");
        }

        page.PageKey = dto.PageKey;
        page.ShowOnHomePage = dto.ShowOnHomePage;
        page.OrderOnHomePage = dto.OrderOnHomePage;
        page.OrderInNavigation = dto.OrderInNavigation;

        foreach (var translationDto in dto.Translations)
        {
            var existingTranslation = page.Translations.FirstOrDefault(t => t.CultureCode == translationDto.CultureCode);

            if (string.IsNullOrWhiteSpace(translationDto.Title) && string.IsNullOrWhiteSpace(translationDto.Slug))
            {
                if (existingTranslation != null)
                {
                    // Optionally remove empty translation? For now, let's just keep it or skip update.
                    // page.Translations.Remove(existingTranslation);
                }
                continue;
            }

            if (existingTranslation == null)
            {
                existingTranslation = new ContentPageTranslationEntity
                {
                    CultureCode = translationDto.CultureCode
                };
                page.Translations.Add(existingTranslation);
            }

            existingTranslation.Title = translationDto.Title;
            existingTranslation.Slug = translationDto.Slug;
            existingTranslation.BodyHtml = translationDto.BodyHtml;
            existingTranslation.BodyDeltaJson = translationDto.BodyDeltaJson;
            existingTranslation.PlainText = StripHtml(translationDto.BodyHtml);
        }

        await Context.SaveChangesAsync();
    }

    public async Task DeletePageAsync(Guid id)
    {
        var page = await Context.ContentPages.FindAsync(id);
        if (page != null)
        {
            page.IsDeleted = true;
            page.DeletedAtUtc = DateTime.UtcNow;
            await Context.SaveChangesAsync();
        }
    }

    private static string? StripHtml(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            return html;
        }

        // Basic HTML stripping using Regex
        var plainText = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]*>", string.Empty);

        // Decode HTML entities
        plainText = System.Net.WebUtility.HtmlDecode(plainText);

        if (plainText.Length > ContentPageTranslationEntity.PlainTextMaxLength)
        {
            plainText = plainText.Substring(0, ContentPageTranslationEntity.PlainTextMaxLength);
        }

        return plainText;
    }
}
