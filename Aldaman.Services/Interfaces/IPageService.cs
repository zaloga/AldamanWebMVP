using Aldaman.Services.Dtos.General;
using Aldaman.Services.Dtos.Page;

namespace Aldaman.Services.Interfaces;

/// <summary>
/// Service for managing system pages and their localized content.
/// </summary>
public interface IPageService
{
    /// <summary>
    /// Gets a page by its URL slug.
    /// </summary>
    Task<PageDetailDto?> GetPageBySlugAsync(string slug, string culture);

    /// <summary>
    /// Gets the home page content.
    /// </summary>
    Task<PageDetailDto?> GetHomePageAsync(string culture);

    /// <summary>
    /// Gets all pages for admin listing with pagination, sorting and filtering.
    /// </summary>
    Task<PagedResultDto<PageListItemDto>> GetPagedPagesAsync(PaginationQuery query);

    /// <summary>
    /// Gets a page for editing in admin.
    /// </summary>
    Task<PageEditDto?> GetPageForEditAsync(Guid id, string culture);

    /// <summary>
    /// Creates a new page.
    /// </summary>
    Task CreatePageAsync(PageEditDto dto);

    /// <summary>
    /// Updates an existing page.
    /// </summary>
    Task UpdatePageAsync(Guid id, PageEditDto dto);

    /// <summary>
    /// Deletes a page and all its contents.
    /// </summary>
    Task DeletePageAsync(Guid id);
}
