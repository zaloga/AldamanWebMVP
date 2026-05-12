using System.Globalization;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Infrastructure.ViewComponents;

public class BrandViewComponent : ViewComponent
{
    private IContentPageService ContentPageService { get; }

    public BrandViewComponent(IContentPageService contentPageService)
    {
        ContentPageService = contentPageService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        string cultureCode = CultureInfo.CurrentUICulture.Name;
        var homePagePages = await ContentPageService.GetHomePageNavigationAsync(cultureCode);
        
        return View(homePagePages);
    }
}
