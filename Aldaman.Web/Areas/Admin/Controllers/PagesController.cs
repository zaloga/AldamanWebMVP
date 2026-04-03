using Aldaman.Services.Dtos.General;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class PagesController : BaseAdminController
{
    private IPageService PageService { get; }

    public PagesController(IPageService pageService)
    {
        PageService = pageService;
    }

    public async Task<IActionResult> Index(PaginationQuery query)
    {
        var result = await PageService.GetPagedPagesAsync(query);
        ViewData["Query"] = query;
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await PageService.DeletePageAsync(id);
            return Json(new { success = true, message = "Page deleted successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error deleting page: " + ex.Message });
        }
    }
}
