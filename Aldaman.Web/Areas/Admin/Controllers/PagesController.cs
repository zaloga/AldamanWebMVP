using Aldaman.Services.Dtos;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class PagesController : BaseAdminController
{
    private readonly IPageService _pageService;

    public PagesController(IPageService pageService)
    {
        _pageService = pageService;
    }

    public async Task<IActionResult> Index(PaginationQuery query)
    {
        var result = await _pageService.GetPagedPagesAsync(query);
        ViewData["Query"] = query;
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _pageService.DeletePageAsync(id);
            return Json(new { success = true, message = "Page deleted successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error deleting page: " + ex.Message });
        }
    }
}
