using Aldaman.Services.Dtos.General;
using Aldaman.Services.Dtos.Media;

namespace Aldaman.Services.Interfaces;

/// <summary>
/// Service for managing media uploads and retrieval.
/// </summary>
public interface IMediaService
{
    /// <summary>
    /// Uploads a file and returns its metadata.
    /// </summary>
    Task<MediaAssetDto> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken ct = default);

    /// <summary>
    /// Lists all media assets with pagination and filtering.
    /// </summary>
    Task<PagedResultDto<MediaAssetDto>> ListAssetsAsync(PaginationQuery query, bool filterDeleted = false, bool onlyImages = false, CancellationToken ct = default);

    /// <summary>
    /// Gets a media asset's metadata by its ID.
    /// </summary>
    Task<MediaAssetDto?> GetAssetAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Updates a media asset's metadata.
    /// </summary>
    Task UpdateAssetAsync(UpdateMediaAssetDto dto, CancellationToken ct = default);

    /// <summary>
    /// Deletes a media asset from the system.
    /// </summary>
    Task DeleteAssetAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Restores a soft-deleted media asset.
    /// </summary>
    Task RestoreAssetAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Permanently deletes a media asset.
    /// </summary>
    Task HardDeleteAssetAsync(Guid id, CancellationToken ct = default);
    
    /// <summary>
    /// Permanently deletes multiple media assets by their relative paths.
    /// </summary>
    Task DeleteMediaAsync(IEnumerable<string> relativePaths, CancellationToken ct = default);
}
