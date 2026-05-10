using Aldaman.Services.Configuration;
using Aldaman.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Aldaman.Web.Infrastructure.ViewComponents;

public sealed class LanguageSwitcherViewComponent : ViewComponent
{
    private readonly LocalizationSettings _settings;

    public LanguageSwitcherViewComponent(IOptions<LocalizationSettings> options)
    {
        _settings = options.Value;
    }

    public IViewComponentResult Invoke()
    {
        string currentCulture = CultureInfo.CurrentUICulture.Name;
        var supportedLanguages = new List<LanguageInfo>();

        // Check for manual overrides from controllers (e.g., translated slugs)
        var alternatives = ViewContext.ViewData["LanguageAlternatives"] as Dictionary<string, string> 
                           ?? new Dictionary<string, string>();

        foreach (var culture in _settings.SupportedCultures)
        {
            string url;

            if (alternatives.TryGetValue(culture, out string? alternativeUrl))
            {
                url = alternativeUrl;
            }
            else
            {
                // Fallback: Swap culture in current route
                var routeValues = new RouteValueDictionary(ViewContext.RouteData.Values);
                routeValues["culture"] = culture;

                // Keep query string if any
                foreach (var query in ViewContext.HttpContext.Request.Query)
                {
                    routeValues[query.Key] = query.Value.ToString();
                }

                url = Url.RouteUrl(routeValues) ?? $"/{culture}";
            }

            supportedLanguages.Add(new LanguageInfo
            {
                Culture = culture,
                DisplayName = culture.ToUpperInvariant(), // Could be expanded to full names like "Čeština", "English"
                Url = url,
                IsActive = string.Equals(culture, currentCulture, StringComparison.OrdinalIgnoreCase)
            });
        }

        var viewModel = new LanguageSwitcherViewModel
        {
            CurrentCulture = currentCulture,
            SupportedLanguages = supportedLanguages
        };

        return View(viewModel);
    }
}
