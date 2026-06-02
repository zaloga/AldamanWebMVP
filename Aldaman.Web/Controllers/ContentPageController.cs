using System.Globalization;
using Aldaman.Services.Configuration;
using Aldaman.Services.Dtos.Page;
using Aldaman.Services.Interfaces;
using Aldaman.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Aldaman.Web.Controllers;

public sealed class ContentPageController : Controller
{
    private IContentPageService ContentPageService { get; }
    private LocalizationSettings LocalizationSettings { get; }

    public ContentPageController(IContentPageService contentPageService, IOptions<LocalizationSettings> localizationOptions)
    {
        ContentPageService = contentPageService;
        LocalizationSettings = localizationOptions.Value;
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
            string defaultCulture = LocalizationSettings.DefaultCulture;
            if (cultureCode != defaultCulture)
            {
                var fallbackSlug = await ContentPageService.GetRedirectSlugAsync(slug, defaultCulture);
                if (fallbackSlug != null)
                {
                    return RedirectToAction("Detail", "ContentPage", new { culture = defaultCulture, slug = fallbackSlug });
                }
            }
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