using Aldaman.Services.Dtos.ContactMessage;
using Aldaman.Services.Interfaces;
using Aldaman.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Controllers;

public sealed class ContactController : Controller
{
    private const string UnknownClientIp = "unknown";
    private const string UnknownUserAgent = "unknown";

    private IContactService ContactService { get; }

    public ContactController(IContactService contactService)
    {
        ContactService = contactService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new ContactFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(
        ContactFormViewModel viewModel,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", viewModel);
        }

        ContactMessageDto dto = new()
        {
            Name = viewModel.Name,
            EmailOrPhone = viewModel.EmailOrPhone,
            Subject = viewModel.Subject,
            Message = viewModel.Message
        };

        string clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? UnknownClientIp;
        string userAgent = string.IsNullOrWhiteSpace(HttpContext.Request.Headers.UserAgent.ToString())
            ? UnknownUserAgent
            : HttpContext.Request.Headers.UserAgent.ToString();

        await ContactService.SubmitMessageAsync(
            dto,
            clientIp,
            userAgent
            /*cancellationToken*/);

        return RedirectToAction(nameof(Success));
    }

    [HttpGet]
    public IActionResult Success()
    {
        return View();
    }
}