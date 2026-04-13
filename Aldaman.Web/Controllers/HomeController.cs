using System.Diagnostics;
using System.Globalization;
using Aldaman.Services.Interfaces;
using Aldaman.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Controllers;

public class HomeController : Controller
{
    private IContentPageService ContentPageService { get; }

    public HomeController(IContentPageService contentPageService)
    {
        ContentPageService = contentPageService;
    }

    public async Task<IActionResult> Index()
    {
        string cultureCode = CultureInfo.CurrentUICulture.Name;
        var homePages = await ContentPageService.GetHomePageAsync(cultureCode);

        return View(homePages);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
