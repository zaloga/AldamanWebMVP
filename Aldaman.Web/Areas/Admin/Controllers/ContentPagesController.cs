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

    public async Task<IActionResult> Index(PaginationQuery query)
    {
        var result = await ContentPageService.GetPagedPagesAsync(query);
        ViewData["Query"] = query;
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await ContentPageService.DeletePageAsync(id);
            return Json(new { success = true, message = "Page deleted successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error deleting page: " + ex.Message });
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        Services.Dtos.Page.ContentPageEditDto model = ContentPageService.GetPageForCreate();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Aldaman.Services.Dtos.Page.ContentPageEditDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await ContentPageService.CreatePageAsync(model);
            TempData["SuccessMessage"] = "Page created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error creating page: " + ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var page = await ContentPageService.GetPageForEditAsync(id);
        if (page == null)
        {
            return NotFound();
        }

        return View(page);
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        var page = await ContentPageService.GetPageForEditAsync(id);
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
            await ContentPageService.UpdatePageAsync(id, model);
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
