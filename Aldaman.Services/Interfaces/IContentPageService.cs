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
    Task<ContentPageDetailDto?> GetPageBySlugAsync(string slug, string culture);

    /// <summary>
    /// Gets the home page contents.
    /// </summary>
    Task<IEnumerable<ContentPageDetailDto>> GetHomePageAsync(string culture);

    /// <summary>
    /// Gets pages marked for display in navigation.
    /// </summary>
    Task<IEnumerable<ContentPageNavigationDto>> GetTopNavigationAsync(string culture);

    /// <summary>
    /// Gets pages marked for display in footer.
    /// </summary>
    Task<IEnumerable<ContentPageNavigationDto>> GetFooternavigationAsync(string culture);

    /// <summary>
    /// Gets all pages for admin listing with pagination, sorting and filtering.
    /// </summary>
    Task<PagedResultDto<ContentPageListItemDto>> GetPagedPagesAsync(PaginationQuery query);

    /// <summary>
    /// Gets a page for editing in admin.
    /// </summary>
    Task<ContentPageEditDto?> GetPageForEditAsync(Guid id);

    /// <summary>
    /// Gets a new page template for creation in admin.
    /// </summary>
    /// <returns>A new page template</returns>
    ContentPageEditDto GetPageForCreate();

    /// <summary>
    /// Creates a new page.
    /// </summary>
    Task CreatePageAsync(ContentPageEditDto dto);

    /// <summary>
    /// Updates an existing page.
    /// </summary>
    Task UpdatePageAsync(Guid id, ContentPageEditDto dto);

    /// <summary>
    /// Deletes a page and all its contents.
    /// </summary>
    Task DeletePageAsync(Guid id);
}
