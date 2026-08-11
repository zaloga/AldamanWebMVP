using Aldaman.Services.Constants;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Dtos.Page;
using Aldaman.Services.Interfaces;
using Aldaman.Services.Resources;
using Aldaman.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class ContentPagesController : BaseAdminController
{
    private IContentPageService ContentPageService { get; }

    public ContentPagesController(IContentPageService contentPageService, IStringLocalizer<UIResources> localizer)
        : base(localizer)
    {
        ContentPageService = contentPageService;
    }

    public async Task<IActionResult> Index([FromQuery] PaginationQuery query)
    {
        string culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
        PagedResultDto<ContentPageListItemDto> result = await ContentPageService.GetPagedContentPagesAsync(query, culture, filterDeleted: false);
        return View(result);
    }

    public async Task<IActionResult> Deleted([FromQuery] PaginationQuery query)
    {
        string culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
        PagedResultDto<ContentPageListItemDto> result = await ContentPageService.GetPagedContentPagesAsync(query, culture, filterDeleted: true);
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await ContentPageService.SoftDeleteContentPageAsync(id);
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
            await ContentPageService.RestoreContentPageAsync(id);
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
            await ContentPageService.HardDeleteContentPageAsync(id);
            return Json(new { success = true, message = Localizer[UIResourceKeys.PermanentlyDeleted].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResourceKeys.ErrorPermanentlyDeleting, ex.Message].Value });
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        Services.Dtos.Page.ContentPageEditDto model = ContentPageService.GetContentPageForCreate();
        return View("Update", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Aldaman.Services.Dtos.Page.ContentPageEditDto model)
    {
        if (!ModelState.IsValid)
        {
            return View("Update", model);
        }

        try
        {
            await ContentPageService.CreateContentPageAsync(model);
            TempData.SetSuccessMessage(Localizer[UIResourceKeys.PageCreatedSuccessfully].Value);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", Localizer[UIResourceKeys.ErrorCreatingPage, ex.Message].Value);
            return View("Update", model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var page = await ContentPageService.GetContentPageForEditAsync(id);
        if (page == null)
        {
            return NotFound();
        }

        return View(page);
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        var page = await ContentPageService.GetContentPageForEditAsync(id);
        if (page == null)
        {
            return NotFound();
        }

        return View(page);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Guid id, Aldaman.Services.Dtos.Page.ContentPageEditDto model)
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
            await ContentPageService.UpdateContentPageAsync(id, model);
            TempData.SetSuccessMessage(Localizer[UIResourceKeys.PageUpdatedSuccessfully].Value);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", Localizer[UIResourceKeys.ErrorUpdatingPage, ex.Message].Value);
            return View(model);
        }
    }
}
