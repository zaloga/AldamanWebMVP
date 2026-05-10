using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Aldaman.Services.Configuration;
using Aldaman.Services.Dtos.Blog;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Helpers;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aldaman.Services.Services;

public sealed class BlogService : IBlogService
{
    private AppDbContext Context { get; }
    private LocalizationSettings Localization { get; }
    private IMediaService MediaService { get; }

    public BlogService(
        AppDbContext context,
        IOptions<LocalizationSettings> localizationOptions,
        IMediaService mediaService)
    {
        Context = context;
        Localization = localizationOptions.Value;
        MediaService = mediaService;
    }

    public async Task<PagedResultDto<BlogPostListItemDto>> GetPagedBlogPostsAdminAsync(PaginationQuery query, string? culture = null)
    {
        var dbQuery = Context.BlogPosts
            .Include(p => p.Translations)
            .Include(p => p.CoverMediaAsset)
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
                ? dbQuery.OrderByDescending(p => p.Translations.Where(t => culture == null || t.CultureCode == culture).Select(t => t.Title).FirstOrDefault())
                : dbQuery.OrderBy(p => p.Translations.Where(t => culture == null || t.CultureCode == culture).Select(t => t.Title).FirstOrDefault()),
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
                Title = p.Translations.FirstOrDefault(t => culture == null || t.CultureCode == culture)!.Title ?? "-",
                Perex = p.Translations.FirstOrDefault(t => culture == null || t.CultureCode == culture)!.Perex ?? "",
                PublishedAtUtc = p.PublishedAtUtc,
                IsPublished = p.IsPublished,
                AuthorName = p.CreatedByUser != null ? p.CreatedByUser.DisplayName : "Unknown",
                CoverImageRelativePath = p.CoverMediaAsset != null ? p.CoverMediaAsset.RelativePath : null,
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

    public async Task<PagedResultDto<BlogPostListItemDto>> GetPagedBlogPostsAsync(int page, int pageSize, string culture)
    {
        var dbQuery = Context.BlogPosts
            .Include(p => p.Translations)
            .Include(p => p.CoverMediaAsset)
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
                CoverImageRelativePath = p.CoverMediaAsset != null ? p.CoverMediaAsset.RelativePath : null,
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

    public async Task<BlogPostDetailDto?> GetBlogPostBySlugAsync(string slug, string culture)
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
            CoverImageRelativePath = post.CoverMediaAsset?.RelativePath,
        };
    }

    public async Task<BlogPostEditDto?> GetBlogPostForEditAsync(Guid id)
    {
        var post = await Context.BlogPosts
            .Include(p => p.Translations)
            .Include(p => p.CoverMediaAsset)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null) return null;

        return new BlogPostEditDto
        {
            Id = post.Id,
            CoverMediaAssetId = post.CoverMediaAssetId,
            CoverImageRelativePath = post.CoverMediaAsset?.RelativePath,
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

    public BlogPostEditDto GetBlogPostForCreate()
    {
        return new BlogPostEditDto
        {
            Translations = Localization.SupportedCultures.Select(c => new BlogPostTranslationDto
            {
                CultureCode = c
            }).ToList()
        };
    }

    public async Task CreateBlogPostAsync(BlogPostEditDto dto)
    {
        var post = new BlogPostEntity
        {
            CoverMediaAssetId = dto.CoverMediaAssetId,
            IsPublished = dto.IsPublished,
            PublishedAtUtc = dto.IsPublished ? (dto.PublishedAtUtc ?? DateTime.UtcNow) : null,
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
                PlainText = StringHelpers.StripHtml(translationDto.BodyHtml, BlogPostTranslationEntity.PlainTextMaxLength)
            };

            post.Translations.Add(translation);
        }

        Context.BlogPosts.Add(post);
        await Context.SaveChangesAsync();
    }

    public async Task UpdateBlogPostAsync(Guid id, BlogPostEditDto dto)
    {
        var post = await Context.BlogPosts
            .IgnoreQueryFilters()
            .Include(p => p.Translations)
            .Include(p => p.CoverMediaAsset)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
        {
            throw new KeyNotFoundException($"Blog post with ID {id} not found.");
        }

        // 1. Capture old state for media cleanup
        var oldMediaPaths = post.Translations.ToDictionary(t => t.CultureCode, t => StringHelpers.ExtractMediaPaths(t.BodyHtml));
        var currentTranslations = post.Translations.ToDictionary(t => t.CultureCode);
        var mediaToDelete = new HashSet<string>();

        // 2. Update core post properties
        // Handle cover image removal or update
        if (dto.RemoveCoverImage)
        {
            post.CoverMediaAssetId = null;
        }
        else if (dto.CoverMediaAssetId.HasValue)
        {
            post.CoverMediaAssetId = dto.CoverMediaAssetId;
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

        // 3. Process translations
        foreach (var translationDto in dto.Translations)
        {
            currentTranslations.TryGetValue(translationDto.CultureCode, out var existingTranslation);

            var title = translationDto.Title?.Trim() ?? string.Empty;
            var slug = translationDto.Slug?.Trim() ?? string.Empty;
            var perex = translationDto.Perex?.Trim() ?? string.Empty;
            var bodyHtml = translationDto.BodyHtml;

            // Determine if the translation is effectively empty
            bool isEmpty = string.IsNullOrWhiteSpace(title) &&
                           string.IsNullOrWhiteSpace(slug) &&
                           string.IsNullOrWhiteSpace(perex) &&
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

                    Context.BlogPostTranslations.Remove(existingTranslation);
                }
                continue;
            }

            // If it doesn't exist in DB, create it
            if (existingTranslation == null)
            {
                existingTranslation = new BlogPostTranslationEntity
                {
                    CultureCode = translationDto.CultureCode,
                    BlogPostId = post.Id
                };
                Context.BlogPostTranslations.Add(existingTranslation);
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

            // Update fields
            existingTranslation.Title = title;
            existingTranslation.Slug = !string.IsNullOrWhiteSpace(slug) ? slug : StringHelpers.ToSlug(title);
            existingTranslation.Perex = translationDto.Perex;
            existingTranslation.BodyHtml = bodyHtml;
            existingTranslation.BodyDeltaJson = translationDto.BodyDeltaJson;
            existingTranslation.PlainText = StringHelpers.StripHtml(bodyHtml, BlogPostTranslationEntity.PlainTextMaxLength);
        }

        // Handle cover image deletion if requested
        if (dto.RemoveCoverImage && post.CoverMediaAsset != null)
        {
            mediaToDelete.Add(post.CoverMediaAsset.RelativePath);
        }

        await Context.SaveChangesAsync();

        // 4. Cleanup orphaned media files
        if (mediaToDelete.Any())
        {
            await MediaService.DeleteMediaAsync(mediaToDelete.ToList());
        }
    }


    public async Task SoftDeleteBlogPostAsync(Guid id)
    {
        var post = await Context.BlogPosts.FindAsync(id);
        if (post != null)
        {
            post.IsDeleted = true;
            await Context.SaveChangesAsync();
        }
    }

    public async Task<PagedResultDto<BlogPostListItemDto>> GetPagedDeletedBlogPostsAsync(PaginationQuery query, string? culture = null)
    {
        var dbQuery = Context.BlogPosts
            .IgnoreQueryFilters()
            .Where(p => p.IsDeleted)
            .Include(p => p.Translations)
            .Include(p => p.CreatedByUser)
            .OrderByDescending(p => p.DeletedAtUtc)
            .AsQueryable();

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new BlogPostListItemDto
            {
                Id = p.Id,
                Title = p.Translations.FirstOrDefault(t => culture == null || t.CultureCode == culture)!.Title
                        ?? p.Translations.FirstOrDefault()!.Title
                        ?? "-",
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

    public async Task RestoreBlogPostAsync(Guid id)
    {
        var post = await Context.BlogPosts.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id);
        if (post != null)
        {
            post.IsDeleted = false;
            post.DeletedAtUtc = null;
            post.DeletedByUserId = null;
            await Context.SaveChangesAsync();
        }
    }

    public async Task HardDeleteBlogPostAsync(Guid id)
    {
        var post = await Context.BlogPosts
            .IgnoreQueryFilters()
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post != null)
        {
            var mediaId = post.CoverMediaAssetId;

            // Explicitly remove translations
            if (post.Translations.Any())
            {
                Context.RemoveRange(post.Translations);
            }

            // Delete post
            Context.BlogPosts.Remove(post);
            await Context.SaveChangesAsync();

            // Try to delete media asset if it exists and is not used elsewhere
            if (mediaId.HasValue)
            {
                var isUsedElsewhere = await Context.BlogPosts.AnyAsync(p => p.CoverMediaAssetId == mediaId.Value);
                if (!isUsedElsewhere)
                {
                    try
                    {
                        await MediaService.HardDeleteAssetAsync(mediaId.Value);
                    }
                    catch
                    {
                        // TODO log the error
                        // Ignore errors during media deletion to avoid failing the post deletion
                    }
                }
            }

            // Remove images used by RTE
            var rteMediaPaths = post.Translations
                .SelectMany(t => StringHelpers.ExtractMediaPaths(t.BodyHtml))
                .ToList();

            if (rteMediaPaths.Any())
            {
                await MediaService.DeleteMediaAsync(rteMediaPaths);
            }
        }
    }
}
