using System.Globalization;
using Aldaman.Services.Dtos.Page;
using Aldaman.Services.Interfaces;
using Aldaman.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Controllers;

public sealed class PageController : Controller
{
    private IPageService PageService { get; }

    public PageController(IPageService pageService)
    {
        PageService = pageService;
    }

    [HttpGet]
    public async Task<IActionResult> Detail(string slug, CancellationToken cancellationToken)
    {
        string cultureCode = CultureInfo.CurrentUICulture.Name;

        PageDetailDto? pageDetail = await PageService.GetPageBySlugAsync(
            slug,
            cultureCode
            /*cancellationToken*/);

        if (pageDetail is null)
        {
            return NotFound();
        }

        PageViewModel viewModel = new()
        {
            Page = pageDetail
        };

        return View(viewModel);
    }
}