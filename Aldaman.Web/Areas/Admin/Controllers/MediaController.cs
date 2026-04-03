using Aldaman.Services.Dtos.General;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class MediaController : BaseAdminController
{
    private IMediaService MediaService { get; }

    public MediaController(IMediaService mediaService)
    {
        MediaService = mediaService;
    }

    public async Task<IActionResult> Index(PaginationQuery query)
    {
        var result = await MediaService.ListAssetsAsync(query);
        ViewData["Query"] = query;
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await MediaService.DeleteAssetAsync(id);
            return Json(new { success = true, message = "Media asset deleted successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error deleting media: " + ex.Message });
        }
    }
}
