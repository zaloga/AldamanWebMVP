using Aldaman.Services.Dtos.AdminDashboard;
using Aldaman.Services.Interfaces;

namespace Aldaman.Services.Services;

public sealed class AdminDashboardService : IAdminDashboardService
{
    public Task<AdminDashboardStatsDto> GetStatsAsync() => throw new NotImplementedException();
}
