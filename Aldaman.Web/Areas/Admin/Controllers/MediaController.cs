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

    public async Task<IActionResult> Index([FromQuery] PaginationQuery query, CancellationToken cancellationToken = default)
    {
        PagedResultDto<MediaAssetDto> result = await MediaService.ListAssetsAsync(query, filterDeleted: false, ct: cancellationToken);
        return View(result);
    }

    public async Task<IActionResult> Deleted([FromQuery] PaginationQuery query, CancellationToken cancellationToken = default)
    {
        PagedResultDto<MediaAssetDto> result = await MediaService.ListAssetsAsync(query, filterDeleted: true, ct: cancellationToken);
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> ApiList([FromQuery] PaginationQuery query, CancellationToken cancellationToken = default)
    {
        PagedResultDto<MediaAssetDto> result = await MediaService.ListAssetsAsync(query, filterDeleted: false, onlyImages: true, ct: cancellationToken);
        return Json(result);
    }

    [HttpGet]
    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("file", Localizer[UIResources.PleaseSelectFile].Value);
            return View();
        }

        try
        {
            using (var stream = file.OpenReadStream())
            {
                await MediaService.UploadAsync(stream, file.FileName, file.ContentType, cancellationToken);
            }

            TempData.SetSuccessMessage(Localizer[UIResources.FileUploadedSuccessfully].Value);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", Localizer[UIResources.ErrorUploadingFile, ex.Message].Value);
            return View();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadQuill(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            return Json(new { success = false, message = Localizer[UIResources.PleaseSelectFile].Value });
        }

        try
        {
            using (var stream = file.OpenReadStream())
            {
                var asset = await MediaService.UploadAsync(stream, file.FileName, file.ContentType, cancellationToken);
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
    public async Task<IActionResult> UploadQuillSkia([FromForm] UploadImageRequest request, CancellationToken cancellationToken = default)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return Json(new { success = false, message = Localizer[UIResources.PleaseSelectFile].Value });
        }

        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = Localizer[UIResources.InvalidRequestPayload].Value });
        }

        await using Stream inputStream = request.File.OpenReadStream();
        byte[] processedBytes = await ImageProcessingService.ProcessImageAsync(inputStream, request.TargetWidth, request.TargetHeight, cancellationToken);

        using MemoryStream processedStream = new(processedBytes);
        string newFileName = Path.ChangeExtension(request.File.FileName, ".webp");
        MediaAssetDto asset = await MediaService.UploadAsync(processedStream, newFileName, "image/webp", cancellationToken);

        return Json(new { success = true, url = asset.RelativePath, alt = asset.AltTextDefault, title = asset.TitleDefault });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadSkia([FromForm] UploadImageRequest request, CancellationToken cancellationToken = default)
    {
        if (request.File == null || request.File.Length == 0)
        {
            ModelState.AddModelError("File", Localizer[UIResources.PleaseSelectFile].Value);
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
        await MediaService.UploadAsync(processedStream, newFileName, "image/webp", cancellationToken);

        TempData.SetSuccessMessage(Localizer[UIResources.FileUploadedSuccessfully].Value);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken = default)
    {
        var asset = await MediaService.GetAssetAsync(id, cancellationToken);
        if (asset == null) return NotFound();

        return View(asset);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken = default)
    {
        var asset = await MediaService.GetAssetAsync(id, cancellationToken);
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
    public async Task<IActionResult> Edit(UpdateMediaAssetDto model, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid) return View(model);

        await MediaService.UpdateAssetAsync(model, cancellationToken);
        TempData.SetSuccessMessage(Localizer[UIResources.MediaMetadataUpdated].Value);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await MediaService.DeleteAssetAsync(id, cancellationToken);
            return Json(new { success = true, message = Localizer[UIResources.DeletedSuccessfully].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResources.ErrorDeleting, ex.Message].Value });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await MediaService.RestoreAssetAsync(id, cancellationToken);
            return Json(new { success = true, message = Localizer[UIResources.RestoredSuccessfully].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResources.ErrorRestoring, ex.Message].Value });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HardDelete(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await MediaService.HardDeleteAssetAsync(id, cancellationToken);
            return Json(new { success = true, message = Localizer[UIResources.PermanentlyDeleted].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResources.ErrorPermanentlyDeleting, ex.Message].Value });
        }
    }
}
