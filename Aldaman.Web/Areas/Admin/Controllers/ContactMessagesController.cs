using Aldaman.Services.Dtos.General;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class ContactMessagesController : BaseAdminController
{
    private IContactService ContactService { get; }

    public ContactMessagesController(IContactService contactService)
    {
        ContactService = contactService;
    }

    public async Task<IActionResult> Index(
        [FromQuery] PaginationQuery query, 
        [FromQuery(Name = "deleted")] PaginationQuery deletedItemsQuery)
    {
        var result = await ContactService.GetPagedMessagesAsync(query);
        var deletedResult = await ContactService.GetPagedDeletedMessagesAsync(deletedItemsQuery);
        
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
            await ContactService.DeleteMessageAsync(id);
            return Json(new { success = true, message = "Message deleted successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error deleting message: " + ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id)
    {
        try
        {
            await ContactService.RestoreMessageAsync(id);
            return Json(new { success = true, message = "Message restored successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error restoring message: " + ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HardDelete(Guid id)
    {
        try
        {
            await ContactService.HardDeleteMessageAsync(id);
            return Json(new { success = true, message = "Message permanently deleted." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error deleting message: " + ex.Message });
        }
    }
}
