using System.Globalization;
using Aldaman.Services.Configuration;
using Aldaman.Services.Dtos.Content;
using Aldaman.Services.Interfaces;
using Aldaman.Web.Extensions;
using Aldaman.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Aldaman.Web.Controllers;

public sealed class ContentController : Controller
{
    private IContentService ContentService { get; }
    private LocalizationSettings LocalizationSettings { get; }
    private ILogger<ContentController> Logger { get; }

    public ContentController(
        IContentService contentService,
        IOptions<LocalizationSettings> localizationOptions,
        ILogger<ContentController> logger)
    {
        ContentService = contentService;
        LocalizationSettings = localizationOptions.Value;
        Logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Detail(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return NotFound();
        }

        string cultureCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        ContentDetailDto? contentDetail = await ContentService.GetContentBySlugCachedAsync(
            slug,
            cultureCode,
            cancellationToken);

        if (contentDetail is null)
        {
            Logger.LogWarning("Requested content does not exist. Slug: {Slug}, CultureCode: {CultureCode}", slug, cultureCode);
            string defaultCulture = LocalizationSettings.DefaultCulture;
            if (cultureCode != defaultCulture)
            {
                var fallbackSlug = await ContentService.GetRedirectSlugCachedAsync(slug, defaultCulture, cancellationToken);
                if (fallbackSlug != null)
                {
                    TempData.SetShowTranslationMissingToast(true);
                    return RedirectToAction("Detail", "Content", new { culture = defaultCulture, slug = fallbackSlug });
                }
            }
            return NotFound();
        }

        // Provide alternative URLs for the language switcher
        var alternativeSlugs = await ContentService.GetAlternativeSlugsCachedAsync(contentDetail.Id, cancellationToken);
        var alternatives = new Dictionary<string, string>();
        foreach (var slugEntry in alternativeSlugs)
        {
            alternatives[slugEntry.Key] = Url.Action("Detail", "Content", new { culture = slugEntry.Key, slug = slugEntry.Value }) ?? $"/{slugEntry.Key}";
        }
        ViewData.SetLanguageAlternatives(alternatives);

        var navigation = await ContentService.GetContentNavigationCachedAsync(contentDetail.Id, cultureCode, cancellationToken);

        ContentViewModel viewModel = new()
        {
            Content = contentDetail,
            PreviousContent = navigation.Previous,
            NextContent = navigation.Next
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetContentBySlug(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return BadRequest();
        }

        string cultureCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        ContentDetailDto? contentDetail = await ContentService.GetContentBySlugCachedAsync(slug, cultureCode, cancellationToken);

        if (contentDetail is null)
        {
            return NotFound();
        }

        return Json(new { bodyHtml = contentDetail.BodyHtml ?? string.Empty });
    }
}
