using System.Diagnostics;
using System.Globalization;
using Aldaman.Services.Interfaces;
using Aldaman.Web.Extensions;
using Aldaman.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Controllers;

public class HomeController : Controller
{
    private IContentGroupService ContentGroupService { get; }

    public HomeController(IContentGroupService contentGroupService)
    {
        ContentGroupService = contentGroupService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        string cultureCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var group = await ContentGroupService.GetHomePageContentGroupCachedAsync(cultureCode, cancellationToken);

        if (group != null)
        {
            var alternativeSlugs = await ContentGroupService.GetAlternativeSlugsCachedAsync(group.Id, cancellationToken);
            var alternatives = new Dictionary<string, string>();
            foreach (var slugEntry in alternativeSlugs)
            {
                alternatives[slugEntry.Key] = $"/{slugEntry.Key}";
            }
            ViewData.SetLanguageAlternatives(alternatives);
        }

        var viewModel = new HomeIndexViewModel
        {
            Group = group
        };

        return View(viewModel);
    }

    public IActionResult Mcp()
    {
        return View();
    }

    public IActionResult Search()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
