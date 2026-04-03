using Aldaman.Services.Dtos;

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

/// <summary>
/// Service for handling contact messages.
/// </summary>
public interface IContactService
{
    /// <summary>
    /// Submits a message from the public web form.
    /// </summary>
    Task SubmitMessageAsync(ContactMessageDto dto, string clientIp, string userAgent);

    /// <summary>
    /// Lists messages with pagination, sorting and filtering.
    /// </summary>
    Task<PagedResultDto<ContactMessageDto>> GetPagedMessagesAsync(PaginationQuery query);

    /// <summary>
    /// Marks a message as handled.
    /// </summary>
    Task MarkAsHandledAsync(Guid id);
    
    /// <summary>
    /// Deletes a message from the system (soft delete).
    /// </summary>
    Task DeleteMessageAsync(Guid id);
}

/// <summary>
/// Service for global site settings.
/// </summary>
public interface ISiteConfigurationService
{
    /// <summary>
    /// Returns the global site configuration.
    /// </summary>
    Task<SiteConfigurationDto> GetConfigurationAsync();

    /// <summary>
    /// Updates the global site configuration.
    /// </summary>
    Task UpdateConfigurationAsync(SiteConfigurationDto dto);
}

/// <summary>
/// Service for the admin dashboard.
/// </summary>
public interface IAdminDashboardService
{
    /// <summary>
    /// Aggregates data for the dashboard.
    /// </summary>
    Task<AdminDashboardStatsDto> GetStatsAsync();
}
