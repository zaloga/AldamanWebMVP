using Aldaman.Services.Interfaces;
using Aldaman.Services.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class HomeController : BaseAdminController
{
    private readonly IAdminDashboardService _adminDashboardService;

    public HomeController(IAdminDashboardService adminDashboardService, IStringLocalizer<UIResources> localizer)
        : base(localizer)
    {
        _adminDashboardService = adminDashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var stats = await _adminDashboardService.GetStatsAsync();
        return View(stats);
    }
}
