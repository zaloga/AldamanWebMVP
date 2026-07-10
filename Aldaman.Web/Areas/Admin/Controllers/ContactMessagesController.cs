using Aldaman.Services.Dtos.ContactMessage;
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
        deletedItemsQuery.SearchTerm = query.SearchTerm;
        if (Request.Query.ContainsKey("SortBy"))
        {
            deletedItemsQuery.SortBy = query.SortBy;
            deletedItemsQuery.SortDescending = query.SortDescending;
        }
        else
        {
            deletedItemsQuery.SortBy = "CreatedAt";
            deletedItemsQuery.SortDescending = true;
        }

        PagedResultDto<ContactMessageDto> result = await ContactService.GetPagedMessagesAsync(query, filterDeleted: false);
        PagedResultDto<ContactMessageDto> deletedResult = await ContactService.GetPagedMessagesAsync(deletedItemsQuery, filterDeleted: true);
        deletedResult.PageParamName = "deleted.Page";

        var model = new PagedResultsDto<ContactMessageDto>
        {
            Items = result,
            DeletedItems = deletedResult,
            Query = query
        };

        return View(model);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var message = await ContactService.GetMessageByIdAsync(id);
        if (message == null)
        {
            return NotFound();
        }

        return View(message);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkHandled(Guid id)
    {
        await ContactService.MarkAsHandledAsync(id);
        return RedirectToAction(nameof(Index));
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
