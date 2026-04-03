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
    Task<MediaAssetDto> UploadAsync(Stream fileStream, string fileName, string contentType);

    /// <summary>
    /// Lists all media assets with pagination and filtering.
    /// </summary>
    Task<PagedResultDto<MediaAssetDto>> ListAssetsAsync(PaginationQuery query);

    /// <summary>
    /// Gets a media asset's metadata by its ID.
    /// </summary>
    Task<MediaAssetDto?> GetAssetAsync(Guid id);

    /// <summary>
    /// Deletes a media asset from the system.
    /// </summary>
    Task DeleteAssetAsync(Guid id);
}
