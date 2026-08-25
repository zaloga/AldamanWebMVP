using Aldaman.Services.Dtos.ContentGroup;
using Aldaman.Services.Dtos.General;

namespace Aldaman.Services.Interfaces;

/// <summary>
/// Service for managing content groups and their localized content.
/// </summary>
public interface IContentGroupService
{
    /// <summary>
    /// Gets all content groups for admin listing with pagination, sorting and filtering.
    /// </summary>
    Task<PagedResultDto<ContentGroupListItemDto>> GetPagedContentGroupsAsync(PaginationQuery query, string? culture = null, bool filterDeleted = false, CancellationToken ct = default);

    /// <summary>
    /// Gets a content group for editing in admin.
    /// </summary>
    Task<ContentGroupEditDto?> GetContentGroupForEditAsync(Guid id, string? culture = null, CancellationToken ct = default);

    /// <summary>
    /// Gets a new content group template for creation in admin with available options.
    /// </summary>
    Task<ContentGroupEditDto> GetContentGroupForCreateAsync(string? culture = null, CancellationToken ct = default);

    /// <summary>
    /// Populates available dropdown options for ContentPages and BlogPosts.
    /// </summary>
    Task PopulateAvailableOptionsAsync(ContentGroupEditDto dto, string? culture = null, CancellationToken ct = default);

    /// <summary>
    /// Creates a new content group.
    /// </summary>
    Task CreateContentGroupAsync(ContentGroupEditDto dto, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing content group.
    /// </summary>
    Task UpdateContentGroupAsync(Guid id, ContentGroupEditDto dto, CancellationToken ct = default);

    /// <summary>
    /// Deletes a content group and its relations (soft-delete).
    /// </summary>
    Task SoftDeleteContentGroupAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Restores a soft-deleted content group.
    /// </summary>
    Task RestoreContentGroupAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Permanently deletes a content group.
    /// </summary>
    Task HardDeleteContentGroupAsync(Guid id, CancellationToken ct = default);
}
