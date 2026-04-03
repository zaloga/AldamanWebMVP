using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class HomeController : BaseAdminController
{
    private readonly IAdminDashboardService _adminDashboardService;

    public HomeController(IAdminDashboardService adminDashboardService)
    {
        _adminDashboardService = adminDashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var stats = await _adminDashboardService.GetStatsAsync();
        return View(stats);
    }
}
