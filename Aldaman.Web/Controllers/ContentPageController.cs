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
    private ILogger<ContentPageController> Logger { get; }

    public ContentPageController(IContentPageService contentPageService, IOptions<LocalizationSettings> localizationOptions, ILogger<ContentPageController> logger)
    {
        ContentPageService = contentPageService;
        LocalizationSettings = localizationOptions.Value;
        Logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Detail(string slug, CancellationToken cancellationToken)
    {
        string cultureCode = CultureInfo.CurrentUICulture.Name;

        ContentPageDetailDto? pageDetail = await ContentPageService.GetContentPageBySlugCachedAsync(
            slug,
            cultureCode);

        if (pageDetail is null)
        {
            Logger.LogWarning("Requested content page does not exist. Slug: {Slug}, CultureCode: {CultureCode}", slug, cultureCode);
            string defaultCulture = LocalizationSettings.DefaultCulture;
            if (cultureCode != defaultCulture)
            {
                var fallbackSlug = await ContentPageService.GetRedirectSlugCachedAsync(slug, defaultCulture);
                if (fallbackSlug != null)
                {
                    TempData["ShowTranslationMissingToast"] = true;
                    return RedirectToAction("Detail", "ContentPage", new { culture = defaultCulture, slug = fallbackSlug });
                }
            }
            return NotFound();
        }

        // Provide alternative URLs for the language switcher
        var alternativeSlugs = await ContentPageService.GetAlternativeSlugsCachedAsync(pageDetail.Id);
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