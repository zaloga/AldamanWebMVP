using Aldaman.Services.Constants;
using Aldaman.Services.Dtos.ContentGroup;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Interfaces;
using Aldaman.Services.Resources;
using Aldaman.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class ContentGroupsController : BaseAdminController
{
    private IContentGroupService ContentGroupService { get; }

    public ContentGroupsController(IContentGroupService contentGroupService, IStringLocalizer<UIResources> localizer)
        : base(localizer)
    {
        ContentGroupService = contentGroupService;
    }

    public async Task<IActionResult> Index([FromQuery] PaginationQuery query, CancellationToken cancellationToken = default)
    {
        string culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
        PagedResultDto<ContentGroupListItemDto> result = await ContentGroupService.GetPagedContentGroupsAsync(query, culture, filterDeleted: false, ct: cancellationToken);
        return View(result);
    }

    public async Task<IActionResult> Deleted([FromQuery] PaginationQuery query, CancellationToken cancellationToken = default)
    {
        string culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
        PagedResultDto<ContentGroupListItemDto> result = await ContentGroupService.GetPagedContentGroupsAsync(query, culture, filterDeleted: true, ct: cancellationToken);
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await ContentGroupService.SoftDeleteContentGroupAsync(id, cancellationToken);
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
            await ContentGroupService.RestoreContentGroupAsync(id, cancellationToken);
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
            await ContentGroupService.HardDeleteContentGroupAsync(id, cancellationToken);
            return Json(new { success = true, message = Localizer[UIResourceKeys.PermanentlyDeleted].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResourceKeys.ErrorPermanentlyDeleting, ex.Message].Value });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
    {
        string culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
        ContentGroupEditDto model = await ContentGroupService.GetContentGroupForCreateAsync(culture, cancellationToken);
        return View("Update", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContentGroupEditDto model, CancellationToken cancellationToken = default)
    {
        string culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
        if (!ModelState.IsValid)
        {
            await ContentGroupService.PopulateAvailableOptionsAsync(model, culture, cancellationToken);
            return View("Update", model);
        }

        try
        {
            await ContentGroupService.CreateContentGroupAsync(model, cancellationToken);
            TempData.SetSuccessMessage(Localizer[UIResourceKeys.ContentGroupCreatedSuccessfully].Value);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, Localizer[UIResourceKeys.ErrorCreatingContentGroup, ex.Message].Value);
            await ContentGroupService.PopulateAvailableOptionsAsync(model, culture, cancellationToken);
            return View("Update", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken = default)
    {
        string culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
        var group = await ContentGroupService.GetContentGroupForEditAsync(id, culture, cancellationToken);
        if (group == null)
        {
            return NotFound();
        }

        return View(group);
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken = default)
    {
        string culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
        var group = await ContentGroupService.GetContentGroupForEditAsync(id, culture, cancellationToken);
        if (group == null)
        {
            return NotFound();
        }

        return View(group);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Guid id, ContentGroupEditDto model, CancellationToken cancellationToken = default)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        string culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
        if (!ModelState.IsValid)
        {
            await ContentGroupService.PopulateAvailableOptionsAsync(model, culture, cancellationToken);
            return View(model);
        }

        try
        {
            await ContentGroupService.UpdateContentGroupAsync(id, model, cancellationToken);
            TempData.SetSuccessMessage(Localizer[UIResourceKeys.ContentGroupUpdatedSuccessfully].Value);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, Localizer[UIResourceKeys.ErrorUpdatingContentGroup, ex.Message].Value);
            await ContentGroupService.PopulateAvailableOptionsAsync(model, culture, cancellationToken);
            return View(model);
        }
    }

}
