using System.Globalization;
using Aldaman.Services.Dtos.Page;
using Aldaman.Services.Interfaces;
using Aldaman.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Controllers;

public sealed class ContentPageController : Controller
{
    private IContentPageService ContentPageService { get; }

    public ContentPageController(IContentPageService contentPageService)
    {
        ContentPageService = contentPageService;
    }

    [HttpGet]
    public async Task<IActionResult> Detail(string slug, CancellationToken cancellationToken)
    {
        string cultureCode = CultureInfo.CurrentUICulture.Name;

        ContentPageDetailDto? pageDetail = await ContentPageService.GetPageBySlugAsync(
            slug,
            cultureCode);

        if (pageDetail is null)
        {
            return NotFound();
        }

        ContentPageViewModel viewModel = new()
        {
            Page = pageDetail
        };

        return View(viewModel);
    }
}