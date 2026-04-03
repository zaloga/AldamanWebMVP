using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Aldaman.Services.Dtos;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aldaman.Services.Services;

public sealed class PageService : IPageService
{
    private readonly AppDbContext _context;

    public PageService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<PageListItemDto>> GetPagedPagesAsync(PaginationQuery query)
    {
        var dbQuery = _context.PageDefinitions
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
        var page = await _context.PageDefinitions
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
        var page = await _context.PageDefinitions
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
        var page = await _context.PageDefinitions
            .Include(p => p.Contents)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (page == null) return null;

        return new PageEditDto
        {
            Id = page.Id,
            PageKey = page.PageKey,
            RouteSegment = page.RouteSegment,
            IsHomePage = page.IsHomePage,
            IsActive = page.IsActive,
            DefaultSortOrder = page.DefaultSortOrder,
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

    public Task CreatePageAsync(PageEditDto dto) => throw new NotImplementedException();
    public Task UpdatePageAsync(Guid id, PageEditDto dto) => throw new NotImplementedException();

    public async Task DeletePageAsync(Guid id)
    {
        var page = await _context.PageDefinitions.FindAsync(id);
        if (page != null)
        {
            page.IsDeleted = true;
            page.DeletedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
