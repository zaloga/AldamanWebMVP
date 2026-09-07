using Aldaman.Services.Dtos.Content;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Interfaces;
using Aldaman.Services.Resources;
using Aldaman.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class ContentsController : BaseAdminController
{
    private IContentService ContentService { get; }
    private IMediaService MediaService { get; }

    public ContentsController(
        IContentService contentService,
        IMediaService mediaService,
        IStringLocalizer<UIResources> localizer)
        : base(localizer)
    {
        ContentService = contentService;
        MediaService = mediaService;
    }

    public async Task<IActionResult> Index([FromQuery] PaginationQuery query, CancellationToken cancellationToken = default)
    {
        string culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
        PagedResultDto<ContentListItemDto> result = await ContentService.GetPagedContentsAdminAsync(query, culture, ct: cancellationToken);
        return View(result);
    }

    public async Task<IActionResult> Deleted([FromQuery] PaginationQuery query, CancellationToken cancellationToken = default)
    {
        string culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
        PagedResultDto<ContentListItemDto> result = await ContentService.GetPagedContentsAdminAsync(query, culture, filterDeleted: true, ct: cancellationToken);
        return View(result);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = ContentService.GetContentForCreate();
        return View("Update", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContentEditDto model, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View("Update", model);
        }

        try
        {
            if (model.CoverImageFile != null && model.CoverImageFile.Length > 0)
            {
                using var stream = model.CoverImageFile.OpenReadStream();
                var asset = await MediaService.UploadAsync(stream, model.CoverImageFile.FileName, model.CoverImageFile.ContentType, cancellationToken);
                model.CoverMediaAssetId = asset.Id;
            }

            await ContentService.CreateContentAsync(model, cancellationToken);
            TempData.SetSuccessMessage(Localizer[UIResources.ContentCreatedSuccessfully].Value);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", Localizer[UIResources.ErrorCreatingContent, ex.Message]);
            return View("Update", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken = default)
    {
        var content = await ContentService.GetContentForEditAsync(id, cancellationToken);
        if (content == null)
        {
            return NotFound();
        }

        return View(content);
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken = default)
    {
        var content = await ContentService.GetContentForEditAsync(id, cancellationToken);
        if (content == null)
        {
            return NotFound();
        }

        return View(content);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Guid id, ContentEditDto model, CancellationToken cancellationToken = default)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            if (model.CoverImageFile != null && model.CoverImageFile.Length > 0)
            {
                using var stream = model.CoverImageFile.OpenReadStream();
                var asset = await MediaService.UploadAsync(stream, model.CoverImageFile.FileName, model.CoverImageFile.ContentType, cancellationToken);
                model.CoverMediaAssetId = asset.Id;
            }

            await ContentService.UpdateContentAsync(id, model, cancellationToken);
            TempData.SetSuccessMessage(Localizer[UIResources.ContentUpdatedSuccessfully].Value);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", Localizer[UIResources.ErrorUpdatingContent, ex.Message]);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await ContentService.SoftDeleteContentAsync(id, cancellationToken);
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
            await ContentService.RestoreContentAsync(id, cancellationToken);
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
            await ContentService.HardDeleteContentAsync(id, cancellationToken);
            return Json(new { success = true, message = Localizer[UIResources.PermanentlyDeleted].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResources.ErrorPermanentlyDeleting, ex.Message].Value });
        }
    }
}
