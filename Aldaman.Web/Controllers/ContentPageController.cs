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

        ContentPageDetailDto? pageDetail = await ContentPageService.GetContentPageBySlugAsync(
            slug,
            cultureCode);

        if (pageDetail is null)
        {
            return NotFound();
        }

        // Provide alternative URLs for the language switcher
        var alternativeSlugs = await ContentPageService.GetAlternativeSlugsAsync(pageDetail.Id);
        var alternatives = new Dictionary<string, string>();
        foreach (var slugEntry in alternativeSlugs)
        {
            alternatives[slugEntry.Key] = Url.Action("Detail", "ContentPage", new { culture = slugEntry.Key, slug = slugEntry.Value }) ?? $"/{slugEntry.Key}";
        }
        ViewData["LanguageAlternatives"] = alternatives;

        ContentPageViewModel viewModel = new()
        {
            Page = pageDetail
        };

        return View(viewModel);
    }
}