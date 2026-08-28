using Aldaman.Services.Dtos.Navigation;

namespace Aldaman.Services.Interfaces;

/// <summary>
/// Service for managing and retrieving navigation elements across the web application.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Gets items marked for display on the home page as navigation links.
    /// </summary>
    Task<IEnumerable<NavigationDto>> GetHomePageNavigationAsync(string culture, CancellationToken ct = default);

    /// <summary>
    /// Gets items marked for display in top navigation.
    /// </summary>
    Task<IEnumerable<NavigationDto>> GetTopNavigationAsync(string culture, CancellationToken ct = default);

    /// <summary>
    /// Gets items marked for display in footer navigation.
    /// </summary>
    Task<IEnumerable<NavigationDto>> GetFooterNavigationAsync(string culture, CancellationToken ct = default);
}
