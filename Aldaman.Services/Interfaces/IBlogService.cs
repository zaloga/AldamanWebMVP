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
    Task<BlogPostDetailDto?> GetBlogPostBySlugAsync(string slug, string culture);

    /// <summary>
    /// Gets a paged list of blog posts for administrative view.
    /// </summary>
    Task<PagedResultDto<BlogPostListItemDto>> GetPagedBlogPostsAdminAsync(PaginationQuery query, string? culture = null);

    /// <summary>
    /// Gets a paged list of blog posts.
    /// </summary>
    Task<PagedResultDto<BlogPostListItemDto>> GetPagedBlogPostsAsync(int page, int pageSize, string culture);

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
    /// Gets a paged list of deleted blog posts.
    /// </summary>
    Task<PagedResultDto<BlogPostListItemDto>> GetPagedDeletedBlogPostsAsync(PaginationQuery query, string? culture = null);

    /// <summary>
    /// Restores a soft-deleted blog post.
    /// </summary>
    Task RestoreBlogPostAsync(Guid id);

    /// <summary>
    /// Permanently deletes a blog post.
    /// </summary>
    Task HardDeleteBlogPostAsync(Guid id);
}
