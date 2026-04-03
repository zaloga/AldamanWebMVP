using Aldaman.Services.Dtos.SiteConfiguration;

namespace Aldaman.Services.Interfaces;

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
