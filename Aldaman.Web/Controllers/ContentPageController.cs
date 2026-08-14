using System.Globalization;
using Aldaman.Services.Configuration;
using Aldaman.Services.Dtos.Page;
using Aldaman.Services.Interfaces;
using Aldaman.Web.Extensions;
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
    public async Task<IActionResult> Detail(string slug, CancellationToken cancellationToken = default)
    {
        string cultureCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        ContentPageDetailDto? pageDetail = await ContentPageService.GetContentPageBySlugCachedAsync(
            slug,
            cultureCode,
            cancellationToken);

        if (pageDetail is null)
        {
            Logger.LogWarning("Requested content page does not exist. Slug: {Slug}, CultureCode: {CultureCode}", slug, cultureCode);
            string defaultCulture = LocalizationSettings.DefaultCulture;
            if (cultureCode != defaultCulture)
            {
                var fallbackSlug = await ContentPageService.GetRedirectSlugCachedAsync(slug, defaultCulture, cancellationToken);
                if (fallbackSlug != null)
                {
                    TempData.SetShowTranslationMissingToast(true);
                    return RedirectToAction("Detail", "ContentPage", new { culture = defaultCulture, slug = fallbackSlug });
                }
            }
            return NotFound();
        }

        // Provide alternative URLs for the language switcher
        var alternativeSlugs = await ContentPageService.GetAlternativeSlugsCachedAsync(pageDetail.Id, cancellationToken);
        var alternatives = new Dictionary<string, string>();
        foreach (var slugEntry in alternativeSlugs)
        {
            alternatives[slugEntry.Key] = Url.Action("Detail", "ContentPage", new { culture = slugEntry.Key, slug = slugEntry.Value }) ?? $"/{slugEntry.Key}";
        }
        ViewData.SetLanguageAlternatives(alternatives);

        ContentPageViewModel viewModel = new()
        {
            Page = pageDetail
        };

        return View(viewModel);
    }
}