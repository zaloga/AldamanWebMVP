using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Dtos.Page;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aldaman.Services.Services;

public sealed class ContentPageService : IContentPageService
{
    private AppDbContext Context { get; }

    public ContentPageService(AppDbContext context)
    {
        Context = context;
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
            "SortOrder" => query.SortDescending ? dbQuery.OrderByDescending(p => p.DefaultSortOrder) : dbQuery.OrderBy(p => p.DefaultSortOrder),
            _ => dbQuery.OrderBy(p => p.DefaultSortOrder)
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
                PageOrder = p.PageOrder,
                DefaultSortOrder = p.DefaultSortOrder,
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
            Slug = content.Slug
        };
    }

    public async Task<IEnumerable<ContentPageDetailDto>> GetHomePageAsync(string culture)
    {
        var pages = await Context.ContentPages
            .Include(p => p.Translations)
            .Where(p => p.ShowOnHomePage)
            .OrderBy(p => p.PageOrder)
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
                Slug = content?.Slug ?? string.Empty
            };
        }).ToList();
    }

    public async Task<ContentPageEditDto?> GetPageForEditAsync(Guid id, string culture)
    {
        var page = await Context.ContentPages
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (page == null) return null;

        var defaultContent = page.Translations.FirstOrDefault(c => c.CultureCode == culture)
                          ?? page.Translations.FirstOrDefault();

        return new ContentPageEditDto
        {
            Id = page.Id,
            PageKey = page.PageKey,
            ShowOnHomePage = page.ShowOnHomePage,
            PageOrder = page.PageOrder,
            DefaultSortOrder = page.DefaultSortOrder,
            CultureCode = defaultContent?.CultureCode ?? culture,
            Title = defaultContent?.Title ?? string.Empty,
            Slug = defaultContent?.Slug ?? string.Empty,
            Translations = page.Translations.Select(c => new ContentPageTranslationDto
            {
                LanguageCode = c.CultureCode,
                Title = c.Title
            }).ToList()
        };
    }

    public async Task CreatePageAsync(ContentPageEditDto dto)
    {
        var page = new ContentPageEntity
        {
            PageKey = dto.PageKey,
            ShowOnHomePage = dto.ShowOnHomePage,
            PageOrder = dto.PageOrder,
            DefaultSortOrder = dto.DefaultSortOrder
        };

        var content = new ContentPageTranslationEntity
        {
            CultureCode = string.IsNullOrWhiteSpace(dto.CultureCode) ? "cs" : dto.CultureCode,
            Title = dto.Title ?? dto.PageKey,
            Slug = !string.IsNullOrWhiteSpace(dto.Slug) ? dto.Slug : dto.PageKey.ToLower().Replace(" ", "-")
        };

        page.Translations.Add(content);

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
        page.PageOrder = dto.PageOrder;
        page.DefaultSortOrder = dto.DefaultSortOrder;

        var culture = string.IsNullOrWhiteSpace(dto.CultureCode) ? "cs" : dto.CultureCode;
        var content = page.Translations.FirstOrDefault(c => c.CultureCode == culture);

        if (content == null)
        {
            content = new ContentPageTranslationEntity
            {
                CultureCode = culture
            };
            page.Translations.Add(content);
        }

        content.Title = dto.Title ?? dto.PageKey;
        content.Slug = !string.IsNullOrWhiteSpace(dto.Slug) ? dto.Slug : content.Slug;

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
}
