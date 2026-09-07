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

    public async Task<IActionResult> Index([FromQuery] PaginationQuery query, CancellationToken cancellationToken = default)
    {
        PagedResultDto<ContactMessageDto> result = await ContactService.GetPagedMessagesAsync(query, filterDeleted: false, ct: cancellationToken);
        return View(result);
    }

    public async Task<IActionResult> Deleted([FromQuery] PaginationQuery query, CancellationToken cancellationToken = default)
    {
        PagedResultDto<ContactMessageDto> result = await ContactService.GetPagedMessagesAsync(query, filterDeleted: true, ct: cancellationToken);
        return View(result);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken = default)
    {
        var message = await ContactService.GetMessageByIdAsync(id, cancellationToken);
        if (message == null)
        {
            return NotFound();
        }

        return View(message);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkHandled(Guid id, CancellationToken cancellationToken = default)
    {
        await ContactService.MarkAsHandledAsync(id, cancellationToken);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await ContactService.DeleteMessageAsync(id, cancellationToken);
            return Json(new { success = true, message = Localizer[UIResources.DeletedSuccessfully].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResources.ErrorDeleting, ex.Message].Value });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await ContactService.RestoreMessageAsync(id, cancellationToken);
            return Json(new { success = true, message = Localizer[UIResources.RestoredSuccessfully].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResources.ErrorRestoring, ex.Message].Value });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HardDelete(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await ContactService.HardDeleteMessageAsync(id, cancellationToken);
            return Json(new { success = true, message = Localizer[UIResources.PermanentlyDeleted].Value });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = Localizer[UIResources.ErrorPermanentlyDeleting, ex.Message].Value });
        }
    }
}
