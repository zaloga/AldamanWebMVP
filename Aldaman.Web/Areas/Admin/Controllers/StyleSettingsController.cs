using Aldaman.Services.Constants;
using Aldaman.Services.Dtos.StyleSettings;
using Aldaman.Services.Interfaces;
using Aldaman.Services.Resources;
using Aldaman.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class StyleSettingsController : BaseAdminController
{
    private readonly IStyleService _styleService;

    public StyleSettingsController(IStyleService styleService, IStringLocalizer<UIResources> localizer)
        : base(localizer)
    {
        _styleService = styleService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var settings = await _styleService.GetAllSettingsAsync(cancellationToken);
        ViewBag.DeletedItems = await _styleService.GetDeletedSettingsAsync(cancellationToken);
        return View(settings);
    }

    [HttpGet]
    public IActionResult Update()
    {
        return View(new UpdateStyleSettingDto { Key = "", Value = "", Type = Aldaman.Persistence.Enums.CssType.Color });
    }

    [HttpGet]
    public async Task<IActionResult> UpdatePage(Guid id, CancellationToken cancellationToken = default)
    {
        var setting = await _styleService.GetSettingByIdAsync(id, cancellationToken);
        if (setting == null) return NotFound();

        var model = new UpdateStyleSettingDto
        {
            Id = setting.Id,
            Key = setting.Key,
            Type = setting.Type,
            Value = setting.Value
        };

        return View("Update", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UpdateStyleSettingDto dto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        bool isEdit = dto.Id.HasValue && dto.Id != Guid.Empty;
        await _styleService.UpdateSettingAsync(dto, cancellationToken);

        TempData.SetSuccessMessage(isEdit
            ? Localizer[UIResourceKeys.StyleSettingUpdated].Value
            : Localizer[UIResourceKeys.StyleSettingCreated].Value);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateInline(Guid id, string value, CancellationToken cancellationToken = default)
    {
        var dto = new UpdateStyleSettingDto
        {
            Id = id,
            Value = value
        };

        await _styleService.UpdateSettingAsync(dto, cancellationToken);

        TempData.SetSuccessMessage(Localizer[UIResourceKeys.StyleSettingUpdated].Value);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetToDefault(Guid id, CancellationToken cancellationToken = default)
    {
        await _styleService.ResetToDefaultSettingAsync(id, cancellationToken);

        return Json(new { success = true, message = Localizer[UIResourceKeys.StyleSettingResetSuccessfully].Value });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _styleService.SoftDeleteSettingAsync(id, cancellationToken);
            return Json(new { success = true, message = Localizer[UIResourceKeys.DeletedSuccessfully].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResourceKeys.ErrorDeleting, ex.Message].Value });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _styleService.RestoreSettingAsync(id, cancellationToken);
            return Json(new { success = true, message = Localizer[UIResourceKeys.RestoredSuccessfully].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResourceKeys.ErrorRestoring, ex.Message].Value });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HardDelete(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _styleService.HardDeleteSettingAsync(id, cancellationToken);
            return Json(new { success = true, message = Localizer[UIResourceKeys.PermanentlyDeleted].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResourceKeys.ErrorPermanentlyDeleting, ex.Message].Value });
        }
    }
}
