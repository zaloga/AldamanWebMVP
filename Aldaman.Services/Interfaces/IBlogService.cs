using Aldaman.Services.Dtos.Blog;
using Aldaman.Services.Dtos.General;

namespace Aldaman.Services.Interfaces;

/// <summary>
/// Service for managing localized blog posts.
/// </summary>
public interface IBlogService
{
    /// <summary>
    /// Gets a blog post by its URL slug.
    /// </summary>
    Task<BlogPostDetailDto?> GetBlogPostBySlugCachedAsync(string slug, string culture);

    /// <summary>
    /// Gets a paged list of blog posts for administrative view.
    /// </summary>
    Task<PagedResultDto<BlogPostListItemDto>> GetPagedBlogPostsAdminAsync(PaginationQuery query, string? culture = null, bool filterDeleted = false);

    /// <summary>
    /// Gets a paged list of blog posts.
    /// </summary>
    Task<PagedResultDto<BlogPostListItemDto>> GetPagedBlogPostsCachedAsync(int page, int pageSize, string culture);

    /// <summary>
    /// Gets a blog post for editing in admin.
    /// </summary>
    Task<BlogPostEditDto?> GetBlogPostForEditAsync(Guid id);

    /// <summary>
    /// Gets an empty blog post for creation in admin.
    /// </summary>
    BlogPostEditDto GetBlogPostForCreate();

    /// <summary>
    /// Creates a new blog post.
    /// </summary>
    Task CreateBlogPostAsync(BlogPostEditDto dto);

    /// <summary>
    /// Updates an existing blog post.
    /// </summary>
    Task UpdateBlogPostAsync(Guid id, BlogPostEditDto dto);

    /// <summary>
    /// Deletes a blog post and all its translations.
    /// </summary>
    Task SoftDeleteBlogPostAsync(Guid id);

    /// <summary>
    /// Restores a soft-deleted blog post.
    /// </summary>
    Task RestoreBlogPostAsync(Guid id);

    /// <summary>
    /// Permanently deletes a blog post.
    /// </summary>
    Task HardDeleteBlogPostAsync(Guid id);

    /// <summary>
    /// Gets slugs for all translations of a blog post.
    /// </summary>
    Task<Dictionary<string, string>> GetAlternativeSlugsCachedAsync(Guid id);

    /// <summary>
    /// Gets navigation information (previous and next post) for a blog post.
    /// </summary>
    Task<(BlogPostNavigationDto? Previous, BlogPostNavigationDto? Next)> GetBlogPostNavigationCachedAsync(Guid currentPostId, string culture);

    /// <summary>
    /// Finds the slug for a published blog post in a target culture if it exists under the given slug in any other culture.
    /// </summary>
    Task<string?> GetRedirectSlugCachedAsync(string slug, string targetCulture);
}
