using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Aldaman.Services.Dtos;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aldaman.Services.Services;

public sealed class BlogService : IBlogService
{
    private readonly AppDbContext _context;

    public BlogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<BlogPostListItemDto>> GetPagedPostsAdminAsync(PaginationQuery query, string? culture = null)
    {
        var dbQuery = _context.BlogPosts
            .Include(p => p.Translations)
            .Include(p => p.CreatedByUser)
            .AsQueryable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            dbQuery = dbQuery.Where(p => p.Translations.Any(t => t.Title.Contains(query.SearchTerm) || (t.Perex != null && t.Perex.Contains(query.SearchTerm))));
        }

        // Sorting
        dbQuery = query.SortBy switch
        {
            "Title" => query.SortDescending 
                ? dbQuery.OrderByDescending(p => p.Translations.FirstOrDefault()!.Title) 
                : dbQuery.OrderBy(p => p.Translations.FirstOrDefault()!.Title),
            "CreatedAt" => query.SortDescending 
                ? dbQuery.OrderByDescending(p => p.CreatedAtUtc) 
                : dbQuery.OrderBy(p => p.CreatedAtUtc),
            "PublishedAt" => query.SortDescending 
                ? dbQuery.OrderByDescending(p => p.PublishedAtUtc) 
                : dbQuery.OrderBy(p => p.PublishedAtUtc),
            _ => dbQuery.OrderByDescending(p => p.CreatedAtUtc)
        };

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new BlogPostListItemDto
            {
                Id = p.Id,
                Title = p.Translations.FirstOrDefault(t => culture == null || t.CultureCode == culture)!.Title ?? "No Title",
                Perex = p.Translations.FirstOrDefault(t => culture == null || t.CultureCode == culture)!.Perex ?? "",
                PublishedAtUtc = p.PublishedAtUtc,
                IsPublished = p.IsPublished,
                AuthorName = p.CreatedByUser != null ? p.CreatedByUser.DisplayName : "Unknown",
                CreatedAtUtc = p.CreatedAtUtc
            })
            .ToListAsync();

        return new PagedResultDto<BlogPostListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<PagedResultDto<BlogPostListItemDto>> GetPagedPostsAsync(int page, int pageSize, string culture)
    {
        var dbQuery = _context.BlogPosts
            .Include(p => p.Translations)
            .Where(p => p.IsPublished && p.Translations.Any(t => t.CultureCode == culture))
            .OrderByDescending(p => p.PublishedAtUtc)
            .AsQueryable();

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new BlogPostListItemDto
            {
                Id = p.Id,
                Title = p.Translations.First(t => t.CultureCode == culture).Title,
                Slug = p.Translations.First(t => t.CultureCode == culture).Slug,
                Perex = p.Translations.First(t => t.CultureCode == culture).Perex,
                PublishedAtUtc = p.PublishedAtUtc,
                IsPublished = p.IsPublished,
                CreatedAtUtc = p.CreatedAtUtc
            })
            .ToListAsync();

        return new PagedResultDto<BlogPostListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<IEnumerable<BlogPostListItemDto>> GetLatestPostsAsync(int count, string culture)
    {
        return (await GetPagedPostsAsync(1, count, culture)).Items;
    }

    public async Task<BlogPostDetailDto?> GetPostBySlugAsync(string slug, string culture)
    {
        var post = await _context.BlogPosts
            .Include(p => p.Translations)
            .Include(p => p.CoverMediaAsset)
            .Include(p => p.CreatedByUser)
            .FirstOrDefaultAsync(p => p.IsPublished && p.Translations.Any(t => t.Slug == slug && t.CultureCode == culture));

        if (post == null) return null;

        var translation = post.Translations.First(t => t.CultureCode == culture);

        return new BlogPostDetailDto
        {
            Id = post.Id,
            Title = translation.Title,
            Perex = translation.Perex,
            BodyHtml = translation.BodyHtml,
            PublishedAtUtc = post.PublishedAtUtc,
            AuthorName = post.CreatedByUser?.DisplayName,
            CoverImageUrl = post.CoverMediaAsset?.RelativePath,
            SeoTitle = translation.SeoTitle,
            SeoDescription = translation.SeoDescription
        };
    }

    public async Task<BlogPostEditDto?> GetPostForEditAsync(Guid id, string culture)
    {
        var post = await _context.BlogPosts
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null) return null;

        return new BlogPostEditDto
        {
            Id = post.Id,
            IsPublished = post.IsPublished,
            PublishedAtUtc = post.PublishedAtUtc,
            Translations = post.Translations.Select(t => new BlogPostTranslationDto
            {
                CultureCode = t.CultureCode,
                Title = t.Title,
                Slug = t.Slug,
                Perex = t.Perex,
                BodyHtml = t.BodyHtml
            }).ToList()
        };
    }

    public async Task SavePostAsync(BlogPostEditDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task DeletePostAsync(Guid id)
    {
        var post = await _context.BlogPosts.FindAsync(id);
        if (post != null)
        {
            post.IsDeleted = true;
            post.DeletedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
