using Aldaman.Services.Dtos.General;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class ContentPagesController : BaseAdminController
{
    private IContentPageService ContentPageService { get; }

    public ContentPagesController(IContentPageService contentPageService)
    {
        ContentPageService = contentPageService;
    }

    public async Task<IActionResult> Index(
        [FromQuery] PaginationQuery query, 
        [FromQuery(Name = "deleted")] PaginationQuery deletedItemsQuery)
    {
        var culture = System.Globalization.CultureInfo.CurrentUICulture.Name;
        var result = await ContentPageService.GetPagedContentPagesAsync(query, culture);
        var deletedResult = await ContentPageService.GetPagedDeletedContentPagesAsync(deletedItemsQuery, culture);
        
        ViewData["Query"] = query;
        ViewData["DeletedQuery"] = deletedItemsQuery;
        ViewBag.DeletedItems = deletedResult;
        
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await ContentPageService.SoftDeleteContentPageAsync(id);
            return Json(new { success = true, message = "Page deleted successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error deleting page: " + ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id)
    {
        try
        {
            await ContentPageService.RestoreContentPageAsync(id);
            return Json(new { success = true, message = "Page restored successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error restoring page: " + ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HardDelete(Guid id)
    {
        try
        {
            await ContentPageService.HardDeleteContentPageAsync(id);
            return Json(new { success = true, message = "Page permanently deleted." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error deleting page: " + ex.Message });
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
            TempData["SuccessMessage"] = "Page created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error creating page: " + ex.Message);
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
            TempData["SuccessMessage"] = "Page updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error updating page: " + ex.Message);
            return View(model);
        }
    }
}
