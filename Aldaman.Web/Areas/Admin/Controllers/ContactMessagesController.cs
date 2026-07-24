using Aldaman.Services.Dtos.ContactMessage;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Interfaces;
using Aldaman.Services.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Aldaman.Web.Areas.Admin.Controllers;

public class ContactMessagesController : BaseAdminController
{
    private IContactService ContactService { get; }

    public ContactMessagesController(IContactService contactService, IStringLocalizer<UIResources> localizer)
        : base(localizer)
    {
        ContactService = contactService;
    }

    public async Task<IActionResult> Index([FromQuery] PaginationQuery query)
    {
        PagedResultDto<ContactMessageDto> result = await ContactService.GetPagedMessagesAsync(query, filterDeleted: false);
        return View(result);
    }

    public async Task<IActionResult> Deleted([FromQuery] PaginationQuery query)
    {
        PagedResultDto<ContactMessageDto> result = await ContactService.GetPagedMessagesAsync(query, filterDeleted: true);
        return View(result);
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
            await ContactService.RestoreMessageAsync(id);
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
            await ContactService.HardDeleteMessageAsync(id);
            return Json(new { success = true, message = Localizer[UIResourceKeys.PermanentlyDeleted].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResourceKeys.ErrorPermanentlyDeleting, ex.Message].Value });
        }
    }
}
