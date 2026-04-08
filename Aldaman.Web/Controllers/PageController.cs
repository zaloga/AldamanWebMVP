using Aldaman.Services.Dtos.Page;
using Aldaman.Services.Interfaces;
using Aldaman.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Controllers;

[Route("stranka")]  // TODO for more languages
public class PageController : Controller
{
    private const string CultureCode = "cs-CZ";  // TODO for more languages
    private IPageService PageService { get; }

    public PageController(IPageService pageService)
    {
        PageService = pageService;
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> Detail(string slug)
    {
        PageDetailDto? pageDetail = await PageService.GetPageBySlugAsync(slug, CultureCode);

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
