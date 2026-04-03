using Aldaman.Services.Dtos.Blog;
using Aldaman.Services.Dtos.General;

namespace Aldaman.Services.Interfaces;

/// <summary>
/// Service for managing localized blog posts.
/// </summary>
public interface IBlogService
{
    /// <summary>
    /// Gets a list of latest posts for the home page.
    /// </summary>
    Task<IEnumerable<BlogPostListItemDto>> GetLatestPostsAsync(int count, string culture);

    /// <summary>
    /// Gets a blog post by its URL slug.
    /// </summary>
    Task<BlogPostDetailDto?> GetPostBySlugAsync(string slug, string culture);

    /// <summary>
    /// Gets a paged list of blog posts for administrative view.
    /// </summary>
    Task<PagedResultDto<BlogPostListItemDto>> GetPagedPostsAdminAsync(PaginationQuery query, string? culture = null);

    /// <summary>
    /// Gets a paged list of blog posts.
    /// </summary>
    Task<PagedResultDto<BlogPostListItemDto>> GetPagedPostsAsync(int page, int pageSize, string culture);

    /// <summary>
    /// Gets a blog post for editing in admin.
    /// </summary>
    Task<BlogPostEditDto?> GetPostForEditAsync(Guid id, string culture);

    /// <summary>
    /// Creates a new blog post.
    /// </summary>
    Task CreatePostAsync(Guid userId, BlogPostEditDto dto);

    /// <summary>
    /// Updates an existing blog post.
    /// </summary>
    Task UpdatePostAsync(Guid id, Guid userId, BlogPostEditDto dto);

    /// <summary>
    /// Deletes a blog post and all its translations.
    /// </summary>
    Task DeletePostAsync(Guid id);
}
