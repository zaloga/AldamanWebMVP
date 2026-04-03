using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Dtos.Page;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aldaman.Services.Services;

public sealed class PageService : IPageService
{
    private AppDbContext Context { get; }

    public PageService(AppDbContext context)
    {
        Context = context;
    }

    public async Task<PagedResultDto<PageListItemDto>> GetPagedPagesAsync(PaginationQuery query)
    {
        var dbQuery = Context.PageDefinitions
            .Include(p => p.Contents)
            .AsQueryable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            dbQuery = dbQuery.Where(p => p.PageKey.Contains(query.SearchTerm) || p.RouteSegment.Contains(query.SearchTerm));
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
            .Select(p => new PageListItemDto
            {
                Id = p.Id,
                PageKey = p.PageKey,
                RouteSegment = p.RouteSegment,
                IsHomePage = p.IsHomePage,
                IsActive = p.IsActive,
                DefaultSortOrder = p.DefaultSortOrder,
                CreatedAtUtc = p.CreatedAtUtc
            })
            .ToListAsync();

        return new PagedResultDto<PageListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<PageDetailDto?> GetPageBySlugAsync(string slug, string culture)
    {
        var page = await Context.PageDefinitions
            .Include(p => p.Contents)
            .FirstOrDefaultAsync(p => p.RouteSegment == slug && p.IsActive);

        if (page == null) return null;

        var content = page.Contents.FirstOrDefault(t => t.CultureCode == culture);
        if (content == null) return null;

        return new PageDetailDto
        {
            Id = page.Id,
            PageKey = page.PageKey,
            Title = content.Title,
            Slug = content.Slug,
            SeoTitle = content.SeoTitle,
            SeoDescription = content.SeoDescription,
            SeoKeywords = content.SeoKeywords,
            SectionsJson = content.SectionsJson
        };
    }

    public async Task<PageDetailDto?> GetHomePageAsync(string culture)
    {
        var page = await Context.PageDefinitions
            .Include(p => p.Contents)
            .FirstOrDefaultAsync(p => p.IsHomePage && p.IsActive);

        if (page == null) return null;

        var content = page.Contents.FirstOrDefault(t => t.CultureCode == culture);
        if (content == null) return null;

        return new PageDetailDto
        {
            Id = page.Id,
            PageKey = page.PageKey,
            Title = content.Title,
            Slug = content.Slug,
            SeoTitle = content.SeoTitle,
            SeoDescription = content.SeoDescription,
            SeoKeywords = content.SeoKeywords,
            SectionsJson = content.SectionsJson
        };
    }

    public async Task<PageEditDto?> GetPageForEditAsync(Guid id, string culture)
    {
        var page = await Context.PageDefinitions
            .Include(p => p.Contents)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (page == null) return null;

        var defaultContent = page.Contents.FirstOrDefault(c => c.CultureCode == culture)
                          ?? page.Contents.FirstOrDefault();

        return new PageEditDto
        {
            Id = page.Id,
            PageKey = page.PageKey,
            RouteSegment = page.RouteSegment,
            IsHomePage = page.IsHomePage,
            IsActive = page.IsActive,
            DefaultSortOrder = page.DefaultSortOrder,
            CultureCode = defaultContent?.CultureCode ?? culture,
            Title = defaultContent?.Title ?? string.Empty,
            Slug = defaultContent?.Slug ?? string.Empty,
            SeoTitle = defaultContent?.SeoTitle,
            SeoDescription = defaultContent?.SeoDescription,
            SeoKeywords = defaultContent?.SeoKeywords,
            IsPublished = defaultContent?.IsPublished ?? false,
            Contents = page.Contents.Select(c => new PageContentDto
            {
                LanguageCode = c.CultureCode,
                Title = c.Title,
                Sections = c.SectionsJson,
                SeoTitle = c.SeoTitle,
                SeoDescription = c.SeoDescription,
                SeoKeywords = c.SeoKeywords
            }).ToList()
        };
    }

    public async Task CreatePageAsync(PageEditDto dto)
    {
        var page = new PageDefinitionEntity
        {
            PageKey = dto.PageKey,
            RouteSegment = dto.RouteSegment,
            IsHomePage = dto.IsHomePage,
            IsActive = dto.IsActive,
            DefaultSortOrder = dto.DefaultSortOrder
        };

        var content = new PageContentEntity
        {
            CultureCode = string.IsNullOrWhiteSpace(dto.CultureCode) ? "cs" : dto.CultureCode,
            Title = dto.Title ?? dto.PageKey,
            Slug = string.IsNullOrWhiteSpace(dto.Slug) ? dto.RouteSegment : dto.Slug,
            SeoTitle = dto.SeoTitle,
            SeoDescription = dto.SeoDescription,
            SeoKeywords = dto.SeoKeywords,
            SectionsJson = "[]",
            IsPublished = dto.IsPublished,
            PublishedAtUtc = dto.IsPublished ? DateTime.UtcNow : null
        };

        page.Contents.Add(content);

        Context.PageDefinitions.Add(page);
        await Context.SaveChangesAsync();
    }

    public async Task UpdatePageAsync(Guid id, PageEditDto dto)
    {
        var page = await Context.PageDefinitions
            .Include(p => p.Contents)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (page == null)
        {
            throw new KeyNotFoundException($"Page with ID {id} not found.");
        }

        page.PageKey = dto.PageKey;
        page.RouteSegment = dto.RouteSegment;
        page.IsHomePage = dto.IsHomePage;
        page.IsActive = dto.IsActive;
        page.DefaultSortOrder = dto.DefaultSortOrder;

        var culture = string.IsNullOrWhiteSpace(dto.CultureCode) ? "cs" : dto.CultureCode;
        var content = page.Contents.FirstOrDefault(c => c.CultureCode == culture);

        if (content == null)
        {
            content = new PageContentEntity
            {
                CultureCode = culture,
                SectionsJson = "[]"
            };
            page.Contents.Add(content);
        }

        content.Title = dto.Title ?? dto.PageKey;
        content.Slug = string.IsNullOrWhiteSpace(dto.Slug) ? dto.RouteSegment : dto.Slug;
        content.SeoTitle = dto.SeoTitle;
        content.SeoDescription = dto.SeoDescription;
        content.SeoKeywords = dto.SeoKeywords;

        if (!content.IsPublished && dto.IsPublished)
        {
            content.PublishedAtUtc = DateTime.UtcNow;
        }
        content.IsPublished = dto.IsPublished;

        await Context.SaveChangesAsync();
    }

    public async Task DeletePageAsync(Guid id)
    {
        var page = await Context.PageDefinitions.FindAsync(id);
        if (page != null)
        {
            page.IsDeleted = true;
            page.DeletedAtUtc = DateTime.UtcNow;
            await Context.SaveChangesAsync();
        }
    }
}
