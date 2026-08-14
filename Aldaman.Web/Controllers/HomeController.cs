using System.Diagnostics;
using System.Globalization;
using Aldaman.Services.Interfaces;
using Aldaman.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Controllers;

public class HomeController : Controller
{
    private const int DefaultPageSize = 10;

    private IContentPageService ContentPageService { get; }
    private IBlogService BlogService { get; }

    public HomeController(IContentPageService contentPageService, IBlogService blogService)
    {
        ContentPageService = contentPageService;
        BlogService = blogService;
    }

    public async Task<IActionResult> Index([FromQuery] int p = 1, CancellationToken cancellationToken = default)
    {
        string cultureCode = CultureInfo.CurrentUICulture.Name;
        var homePages = await ContentPageService.GetHomePageCachedAsync(cultureCode, cancellationToken);
        var blogPosts = await BlogService.GetPagedBlogPostsCachedAsync(p, DefaultPageSize, cultureCode, cancellationToken);

        var viewModel = new HomeIndexViewModel
        {
            HomePages = homePages,
            Posts = blogPosts
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
