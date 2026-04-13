using Aldaman.Services.Dtos.General;
using Aldaman.Services.Dtos.Media;
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

    [HttpGet]
    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("file", "Please select a file to upload.");
            return View();
        }

        try
        {
            using (var stream = file.OpenReadStream())
            {
                await MediaService.UploadAsync(stream, file.FileName, file.ContentType);
            }

            TempData["SuccessMessage"] = "File uploaded successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error uploading file: " + ex.Message);
            return View();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var asset = await MediaService.GetAssetAsync(id);
        if (asset == null) return NotFound();

        var model = new UpdateMediaAssetDto
        {
            Id = asset.Id,
            AltText = asset.AltText,
            Title = asset.Title
        };

        ViewData["RelativePath"] = asset.RelativePath;
        ViewData["IsImage"] = asset.IsImage;

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateMediaAssetDto model)
    {
        if (!ModelState.IsValid) return View(model);

        await MediaService.UpdateAssetAsync(model);
        TempData["SuccessMessage"] = "Media metadata updated.";
        return RedirectToAction(nameof(Index));
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
