using Aldaman.Services.Dtos.AdminDashboard;

namespace Aldaman.Services.Interfaces;

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
