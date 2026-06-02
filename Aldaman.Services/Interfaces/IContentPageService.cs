using Aldaman.Services.Dtos.General;
using Aldaman.Services.Dtos.Page;

namespace Aldaman.Services.Interfaces;

/// <summary>
/// Service for managing system pages and their localized content.
/// </summary>
public interface IContentPageService
{
    /// <summary>
    /// Gets a page by its URL slug.
    /// </summary>
    Task<ContentPageDetailDto?> GetContentPageBySlugCachedAsync(string slug, string culture);

    /// <summary>
    /// Gets the home page contents.
    /// </summary>
    Task<IEnumerable<ContentPageDetailDto>> GetHomePageCachedAsync(string culture);

    /// <summary>
    /// Gets pages marked for display on home page as navigation links.
    /// </summary>
    Task<IEnumerable<ContentPageNavigationDto>> GetHomePageNavigationAsync(string culture);

    /// <summary>
    /// Gets pages marked for display in navigation.
    /// </summary>
    Task<IEnumerable<ContentPageNavigationDto>> GetTopNavigationAsync(string culture);

    /// <summary>
    /// Gets pages marked for display in footer.
    /// </summary>
    Task<IEnumerable<ContentPageNavigationDto>> GetFooterNavigationAsync(string culture);

    /// <summary>
    /// Gets all pages for admin listing with pagination, sorting and filtering.
    /// </summary>
    Task<PagedResultDto<ContentPageListItemDto>> GetPagedContentPagesCachedAsync(PaginationQuery query, string? culture = null);

    /// <summary>
    /// Gets a page for editing in admin.
    /// </summary>
    Task<ContentPageEditDto?> GetContentPageForEditAsync(Guid id);

    /// <summary>
    /// Gets a new page template for creation in admin.
    /// </summary>
    /// <returns>A new page template</returns>
    ContentPageEditDto GetContentPageForCreate();

    /// <summary>
    /// Creates a new page.
    /// </summary>
    Task CreateContentPageAsync(ContentPageEditDto dto);

    /// <summary>
    /// Updates an existing page.
    /// </summary>
    Task UpdateContentPageAsync(Guid id, ContentPageEditDto dto);

    /// <summary>
    /// Deletes a page and all its contents.
    /// </summary>
    Task SoftDeleteContentPageAsync(Guid id);

    /// <summary>
    /// Gets a paged list of deleted pages.
    /// </summary>
    Task<PagedResultDto<ContentPageListItemDto>> GetPagedDeletedContentPagesAsync(PaginationQuery query, string? culture = null);

    /// <summary>
    /// Restores a soft-deleted page.
    /// </summary>
    Task RestoreContentPageAsync(Guid id);

    /// <summary>
    /// Permanently deletes a page.
    /// </summary>
    Task HardDeleteContentPageAsync(Guid id);

    /// <summary>
    /// Gets slugs for all translations of a page.
    /// </summary>
    Task<Dictionary<string, string>> GetAlternativeSlugsCachedAsync(Guid id);

    /// <summary>
    /// Finds the slug for a page in a target culture if it exists under the given slug in any other culture.
    /// </summary>
    Task<string?> GetRedirectSlugCachedAsync(string slug, string targetCulture);

    /// <summary>
    /// Gets a paged list of content pages for admin listing with pagination, sorting and filtering, without caching.
    /// </summary>
    Task<PagedResultDto<ContentPageListItemDto>> GetPagedContentPagesAsync(PaginationQuery query, string? culture = null);
}
