using Aldaman.Services.Dtos.StyleSettings;
using Aldaman.Services.Interfaces;
using Aldaman.Services.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Aldaman.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class StyleSettingsController : Controller // TODO refaktornout do BaseAdminController a přesunout logiku do services
{
    private readonly IStyleService _styleService;
    private IStringLocalizer<UIResources> Localizer { get; }

    public StyleSettingsController(IStyleService styleService, IStringLocalizer<UIResources> localizer)
    {
        _styleService = styleService;
        Localizer = localizer;
    }

    public async Task<IActionResult> Index()
    {
        var settings = await _styleService.GetAllSettingsAsync();
        ViewBag.DeletedItems = await _styleService.GetDeletedSettingsAsync();
        return View(settings);
    }

    [HttpGet]
    public IActionResult Update()
    {
        return View(new UpdateStyleSettingDto { Key = "", Value = "", Type = Aldaman.Persistence.Enums.CssType.Color });
    }

    [HttpGet]
    public async Task<IActionResult> UpdatePage(Guid id)
    {
        var setting = await _styleService.GetSettingByIdAsync(id);
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
    public async Task<IActionResult> Update(UpdateStyleSettingDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        bool isEdit = dto.Id.HasValue && dto.Id != Guid.Empty;
        await _styleService.UpdateSettingAsync(dto);

        TempData["SuccessMessage"] = isEdit
            ? "Style setting updated successfully."
            : "Style setting created successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateInline(Guid id, string value)
    {
        var dto = new UpdateStyleSettingDto
        {
            Id = id,
            Value = value
        };

        await _styleService.UpdateSettingAsync(dto);

        TempData["SuccessMessage"] = "Style setting updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetToDefault(Guid id)
    {
        await _styleService.ResetToDefaultSettingAsync(id);

        return Json(new { success = true, message = "Style setting reset to default successfully." });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _styleService.SoftDeleteSettingAsync(id);
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
            await _styleService.RestoreSettingAsync(id);
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
            await _styleService.HardDeleteSettingAsync(id);
            return Json(new { success = true, message = Localizer[UIResourceKeys.PermanentlyDeleted].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResourceKeys.ErrorPermanentlyDeleting, ex.Message].Value });
        }
    }
}
