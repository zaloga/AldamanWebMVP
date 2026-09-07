using Aldaman.Services.Dtos.Content;
using Aldaman.Services.Dtos.General;

namespace Aldaman.Services.Interfaces;

/// <summary>
/// Service for managing unified content entities (pages and articles) and their localized translations.
/// </summary>
public interface IContentService
{
    #region Admin web part methods

    /// <summary>
    /// Gets all content items for admin listing with pagination, sorting and filtering.
    /// </summary>
    Task<PagedResultDto<ContentListItemDto>> GetPagedContentsAdminAsync(PaginationQuery query, string? culture = null, bool filterDeleted = false, CancellationToken ct = default);

    /// <summary>
    /// Gets a content item for editing in admin.
    /// </summary>
    Task<ContentEditDto?> GetContentForEditAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Gets a new content item template for creation in admin.
    /// </summary>
    ContentEditDto GetContentForCreate();

    /// <summary>
    /// Creates a new content item.
    /// </summary>
    Task CreateContentAsync(ContentEditDto dto, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing content item.
    /// </summary>
    Task UpdateContentAsync(Guid id, ContentEditDto dto, CancellationToken ct = default);

    /// <summary>
    /// Soft deletes a content item.
    /// </summary>
    Task SoftDeleteContentAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Restores a soft-deleted content item.
    /// </summary>
    Task RestoreContentAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Permanently deletes a content item.
    /// </summary>
    Task HardDeleteContentAsync(Guid id, CancellationToken ct = default);

    #endregion

    #region Public web part methods

    /// <summary>
    /// Gets a paged list of published content items for public web display.
    /// </summary>
    Task<PagedResultDto<ContentListItemDto>> GetPagedContentsCachedAsync(int page, int pageSize, string culture, CancellationToken ct = default);

    /// <summary>
    /// Gets a content item by its URL slug.
    /// </summary>
    Task<ContentDetailDto?> GetContentBySlugCachedAsync(string slug, string culture, CancellationToken ct = default);

    /// <summary>
    /// Gets the home page contents.
    /// </summary>
    Task<IEnumerable<ContentDetailDto>> GetHomePageCachedAsync(string culture, CancellationToken ct = default);

    /// <summary>
    /// Gets items marked for display in the right sidebar as banners.
    /// </summary>
    Task<IEnumerable<ContentDetailDto>> GetSidebarBannersCachedAsync(string culture, CancellationToken ct = default);

    /// <summary>
    /// Gets slugs for all translations of a content item.
    /// </summary>
    Task<Dictionary<string, string>> GetAlternativeSlugsCachedAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Gets navigation information (previous and next content) for a content item.
    /// </summary>
    Task<(ContentNavigationDto? Previous, ContentNavigationDto? Next)> GetContentNavigationCachedAsync(Guid currentContentId, string culture, CancellationToken ct = default);

    /// <summary>
    /// Finds the slug for a content item in a target culture if it exists under the given slug in any other culture.
    /// </summary>
    Task<string?> GetRedirectSlugCachedAsync(string slug, string targetCulture, CancellationToken ct = default);

    #endregion
}
