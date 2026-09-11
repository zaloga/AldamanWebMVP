using System.Globalization;
using Aldaman.Services.Configuration;
using Aldaman.Services.Dtos.ContentGroup;
using Aldaman.Services.Interfaces;
using Aldaman.Web.Constants;
using Aldaman.Web.Extensions;
using Aldaman.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Aldaman.Web.Controllers;

public sealed class ContentGroupController : Controller
{
    private IContentGroupService ContentGroupService { get; }
    private LocalizationSettings LocalizationSettings { get; }
    private ILogger<ContentGroupController> Logger { get; }

    public ContentGroupController(
        IContentGroupService contentGroupService,
        IOptions<LocalizationSettings> localizationOptions,
        ILogger<ContentGroupController> logger)
    {
        ContentGroupService = contentGroupService;
        LocalizationSettings = localizationOptions.Value;
        Logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Detail(string slug, CancellationToken cancellationToken = default)
    {
        string cultureCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        ContentGroupDetailDto? groupDetail = await ContentGroupService.GetContentGroupBySlugCachedAsync(
            slug,
            cultureCode,
            cancellationToken);

        if (groupDetail is null)
        {
            Logger.LogWarning("Requested content group does not exist. Slug: {Slug}, CultureCode: {CultureCode}", slug, cultureCode);
            string defaultCulture = LocalizationSettings.DefaultCulture;
            if (cultureCode != defaultCulture)
            {
                var fallbackSlug = await ContentGroupService.GetRedirectSlugCachedAsync(slug, defaultCulture, cancellationToken);
                if (fallbackSlug != null)
                {
                    TempData.SetShowTranslationMissingToast(true);
                    return RedirectToRoute(RouteConstants.ContentGroupDetail, new { culture = defaultCulture, slug = fallbackSlug });
                }
            }
            return NotFound();
        }

        // Provide alternative URLs for the language switcher
        var alternativeSlugs = await ContentGroupService.GetAlternativeSlugsCachedAsync(groupDetail.Id, cancellationToken);
        var alternatives = new Dictionary<string, string>();
        foreach (var slugEntry in alternativeSlugs)
        {
            alternatives[slugEntry.Key] = Url.RouteUrl(RouteConstants.ContentGroupDetail, new { culture = slugEntry.Key, slug = slugEntry.Value }) ?? $"/{slugEntry.Key}";
        }
        ViewData.SetLanguageAlternatives(alternatives);

        ContentGroupViewModel viewModel = new()
        {
            Group = groupDetail
        };

        return View(viewModel);
    }
}
