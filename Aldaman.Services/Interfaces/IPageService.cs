using Aldaman.Services.Dtos;

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
    /// Gets all pages for admin listing.
    /// </summary>
    Task<IEnumerable<PageListItemDto>> GetPagesAsync();

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
