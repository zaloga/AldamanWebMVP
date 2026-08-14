using Aldaman.Services.Dtos.General;
using Aldaman.Services.Dtos.Media;
using Aldaman.Services.Interfaces;
using Aldaman.Services.Resources;
using Aldaman.Services.Services.Images;
using Aldaman.Web.Extensions;
using Aldaman.Web.Models.Media;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class MediaController : BaseAdminController
{
    private IMediaService MediaService { get; }
    private IImageProcessingService ImageProcessingService { get; }

    public MediaController(
        IMediaService mediaService,
        IImageProcessingService imageProcessingService,
        IStringLocalizer<UIResources> localizer)
        : base(localizer)
    {
        MediaService = mediaService;
        ImageProcessingService = imageProcessingService;
    }

    public async Task<IActionResult> Index([FromQuery] PaginationQuery query)
    {
        PagedResultDto<MediaAssetDto> result = await MediaService.ListAssetsAsync(query, filterDeleted: false);
        return View(result);
    }

    public async Task<IActionResult> Deleted([FromQuery] PaginationQuery query)
    {
        PagedResultDto<MediaAssetDto> result = await MediaService.ListAssetsAsync(query, filterDeleted: true);
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> ApiList([FromQuery] PaginationQuery query)
    {
        PagedResultDto<MediaAssetDto> result = await MediaService.ListAssetsAsync(query, filterDeleted: false, onlyImages: true);
        return Json(result);
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
            ModelState.AddModelError("file", Localizer[UIResourceKeys.PleaseSelectFile].Value);
            return View();
        }

        try
        {
            using (var stream = file.OpenReadStream())
            {
                await MediaService.UploadAsync(stream, file.FileName, file.ContentType);
            }

            TempData.SetSuccessMessage(Localizer[UIResourceKeys.FileUploadedSuccessfully].Value);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", Localizer[UIResourceKeys.ErrorUploadingFile, ex.Message].Value);
            return View();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadQuill(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return Json(new { success = false, message = Localizer[UIResourceKeys.PleaseSelectFile].Value });
        }

        try
        {
            using (var stream = file.OpenReadStream())
            {
                var asset = await MediaService.UploadAsync(stream, file.FileName, file.ContentType);
                return Json(new { success = true, url = asset.RelativePath, alt = asset.AltTextDefault, title = asset.TitleDefault });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadQuillSkia([FromForm] UploadImageRequest request, CancellationToken cancellationToken)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return Json(new { success = false, message = Localizer[UIResourceKeys.PleaseSelectFile].Value });
        }

        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = Localizer[UIResourceKeys.InvalidRequestPayload].Value });
        }

        await using Stream inputStream = request.File.OpenReadStream();
        byte[] processedBytes = await ImageProcessingService.ProcessImageAsync(inputStream, request.TargetWidth, request.TargetHeight, cancellationToken);

        using MemoryStream processedStream = new(processedBytes);
        string newFileName = Path.ChangeExtension(request.File.FileName, ".webp");
        MediaAssetDto asset = await MediaService.UploadAsync(processedStream, newFileName, "image/webp");

        return Json(new { success = true, url = asset.RelativePath, alt = asset.AltTextDefault, title = asset.TitleDefault });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadSkia([FromForm] UploadImageRequest request, CancellationToken cancellationToken)
    {
        if (request.File == null || request.File.Length == 0)
        {
            ModelState.AddModelError("File", Localizer[UIResourceKeys.PleaseSelectFile].Value);
            return View("Upload");
        }

        if (!ModelState.IsValid)
        {
            return View("Upload");
        }

        await using Stream inputStream = request.File.OpenReadStream();
        byte[] processedBytes = await ImageProcessingService.ProcessImageAsync(inputStream, request.TargetWidth, request.TargetHeight, cancellationToken);

        using MemoryStream processedStream = new(processedBytes);
        string newFileName = Path.ChangeExtension(request.File.FileName, ".webp");
        await MediaService.UploadAsync(processedStream, newFileName, "image/webp");

        TempData.SetSuccessMessage(Localizer[UIResourceKeys.FileUploadedSuccessfully].Value);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var asset = await MediaService.GetAssetAsync(id);
        if (asset == null) return NotFound();

        return View(asset);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var asset = await MediaService.GetAssetAsync(id);
        if (asset == null) return NotFound();

        var model = new UpdateMediaAssetDto
        {
            Id = asset.Id,
            AltTextDefault = asset.AltTextDefault,
            TitleDefault = asset.TitleDefault
        };

        ViewData.SetMediaPreview(asset.RelativePath, asset.IsImage);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateMediaAssetDto model)
    {
        if (!ModelState.IsValid) return View(model);

        await MediaService.UpdateAssetAsync(model);
        TempData.SetSuccessMessage(Localizer[UIResourceKeys.MediaMetadataUpdated].Value);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await MediaService.DeleteAssetAsync(id);
            return Json(new { success = true, message = Localizer[UIResourceKeys.DeletedSuccessfully].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResourceKeys.ErrorDeleting, ex.Message].Value });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id)
    {
        try
        {
            await MediaService.RestoreAssetAsync(id);
            return Json(new { success = true, message = Localizer[UIResourceKeys.RestoredSuccessfully].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResourceKeys.ErrorRestoring, ex.Message].Value });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HardDelete(Guid id)
    {
        try
        {
            await MediaService.HardDeleteAssetAsync(id);
            return Json(new { success = true, message = Localizer[UIResourceKeys.PermanentlyDeleted].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResourceKeys.ErrorPermanentlyDeleting, ex.Message].Value });
        }
    }
}
