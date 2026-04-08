using Aldaman.Services.Dtos.ContactMessage;
using Aldaman.Services.Interfaces;
using Aldaman.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Controllers;

[Route("kontakt")]  // TODO for more languages
public class ContactController : Controller
{
    private IContactService ContactService { get; }

    public ContactController(IContactService contactService)
    {
        ContactService = contactService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        ContactFormViewModel viewModel = new();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(ContactFormViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", viewModel);
        }

        ContactMessageDto dto = new()
        {
            Name = viewModel.Name,
            Email = viewModel.Email,
            Subject = viewModel.Subject,
            Message = viewModel.Message
        };

        string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"; // TODO global constant
        string userAgent = HttpContext.Request.Headers.UserAgent.ToString() ?? "unknown";

        await ContactService.SubmitMessageAsync(dto, clientIp, userAgent);

        return RedirectToAction(nameof(Success));
    }

    [HttpGet("uspech")]
    public IActionResult Success()
    {
        return View();
    }
}
