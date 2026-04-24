using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Aldaman.Services.Configuration;
using Aldaman.Services.Dtos.Blog;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aldaman.Services.Services;

public sealed class BlogService : IBlogService
{
    private AppDbContext Context { get; }
    private LocalizationSettings Localization { get; }

    public BlogService(AppDbContext context, IOptions<LocalizationSettings> localizationOptions)
    {
        Context = context;
        Localization = localizationOptions.Value;
    }

    public async Task<PagedResultDto<BlogPostListItemDto>> GetPagedPostsAdminAsync(PaginationQuery query, string? culture = null)
    {
        var dbQuery = Context.BlogPosts
            .Include(p => p.Translations)
            .Include(p => p.CreatedByUser)
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
        var dbQuery = Context.BlogPosts
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
        var post = await Context.BlogPosts
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
            BodyDeltaJson = translation.BodyDeltaJson,
            PlainText = translation.PlainText,
            PublishedAtUtc = post.PublishedAtUtc,
            AuthorName = post.CreatedByUser?.DisplayName,
            CoverImageUrl = post.CoverMediaAsset?.RelativePath,
        };
    }

    public async Task<BlogPostEditDto?> GetPostForEditAsync(Guid id)
    {
        var post = await Context.BlogPosts
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null) return null;

        return new BlogPostEditDto
        {
            Id = post.Id,
            IsPublished = post.IsPublished,
            PublishedAtUtc = post.PublishedAtUtc,
            Translations = Localization.SupportedCultures.Select(culture =>
            {
                var translation = post.Translations.FirstOrDefault(t => t.CultureCode == culture);
                return new BlogPostTranslationDto
                {
                    CultureCode = culture,
                    Title = translation?.Title ?? string.Empty,
                    Slug = translation?.Slug ?? string.Empty,
                    Perex = translation?.Perex,
                    BodyHtml = translation?.BodyHtml,
                    BodyDeltaJson = translation?.BodyDeltaJson,
                    PlainText = translation?.PlainText
                };
            }).ToList()
        };
    }

    public BlogPostEditDto GetPostForCreate()
    {
        return new BlogPostEditDto
        {
            Translations = Localization.SupportedCultures.Select(c => new BlogPostTranslationDto
            {
                CultureCode = c
            }).ToList()
        };
    }

    public async Task CreatePostAsync(Guid userId, BlogPostEditDto dto)
    {
        var post = new BlogPostEntity
        {
            IsPublished = dto.IsPublished,
            PublishedAtUtc = dto.IsPublished ? (dto.PublishedAtUtc ?? DateTime.UtcNow) : null,
            CreatedByUserId = userId,
            UpdatedByUserId = userId,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        foreach (var translationDto in dto.Translations)
        {
            if (string.IsNullOrWhiteSpace(translationDto.Title) && string.IsNullOrWhiteSpace(translationDto.Slug))
                continue;

            var translation = new BlogPostTranslationEntity
            {
                CultureCode = translationDto.CultureCode,
                Title = translationDto.Title,
                Slug = !string.IsNullOrWhiteSpace(translationDto.Slug)
                    ? translationDto.Slug
                    : translationDto.Title.ToLower().Replace(" ", "-"),
                Perex = translationDto.Perex,
                BodyHtml = translationDto.BodyHtml,
                BodyDeltaJson = translationDto.BodyDeltaJson,
                PlainText = StripHtml(translationDto.BodyHtml)
            };

            post.Translations.Add(translation);
        }

        Context.BlogPosts.Add(post);
        await Context.SaveChangesAsync();
    }

    public async Task UpdatePostAsync(Guid id, Guid userId, BlogPostEditDto dto)
    {
        var post = await Context.BlogPosts
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
        {
            throw new KeyNotFoundException($"Blog post with ID {id} not found.");
        }

        post.IsPublished = dto.IsPublished;
        if (dto.IsPublished && !post.PublishedAtUtc.HasValue)
        {
            post.PublishedAtUtc = dto.PublishedAtUtc ?? DateTime.UtcNow;
        }
        else if (!dto.IsPublished)
        {
            post.PublishedAtUtc = null;
        }

        post.UpdatedByUserId = userId;
        post.UpdatedAtUtc = DateTime.UtcNow;

        foreach (var translationDto in dto.Translations)
        {
            var existingTranslation = post.Translations.FirstOrDefault(t => t.CultureCode == translationDto.CultureCode);

            if (string.IsNullOrWhiteSpace(translationDto.Title) && string.IsNullOrWhiteSpace(translationDto.Slug))
            {
                continue;
            }

            if (existingTranslation == null)
            {
                existingTranslation = new BlogPostTranslationEntity
                {
                    CultureCode = translationDto.CultureCode
                };
                post.Translations.Add(existingTranslation);
            }

            existingTranslation.Title = translationDto.Title;
            existingTranslation.Slug = translationDto.Slug;
            existingTranslation.Perex = translationDto.Perex;
            existingTranslation.BodyHtml = translationDto.BodyHtml;
            existingTranslation.BodyDeltaJson = translationDto.BodyDeltaJson;
            existingTranslation.PlainText = StripHtml(translationDto.BodyHtml);
        }

        await Context.SaveChangesAsync();
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

        if (plainText.Length > BlogPostTranslationEntity.PlainTextMaxLength)
        {
            plainText = plainText.Substring(0, BlogPostTranslationEntity.PlainTextMaxLength);
        }

        return plainText;
    }

    public async Task DeletePostAsync(Guid id)
    {
        var post = await Context.BlogPosts.FindAsync(id);
        if (post != null)
        {
            post.IsDeleted = true;
            post.DeletedAtUtc = DateTime.UtcNow;
            await Context.SaveChangesAsync();
        }
    }
}
