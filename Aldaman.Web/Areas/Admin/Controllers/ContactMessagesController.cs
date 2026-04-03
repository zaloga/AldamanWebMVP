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

    public async Task<IActionResult> Index(PaginationQuery query)
    {
        var result = await ContactService.GetPagedMessagesAsync(query);
        ViewData["Query"] = query;
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
}
