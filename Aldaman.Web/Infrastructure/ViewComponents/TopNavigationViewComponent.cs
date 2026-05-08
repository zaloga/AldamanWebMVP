using System.Globalization;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Aldaman.Web.ViewModels;

namespace Aldaman.Web.Infrastructure.ViewComponents;

public class TopNavigationViewComponent : ViewComponent
{
    private IContentPageService ContentPageService { get; }

    public TopNavigationViewComponent(IContentPageService contentPageService)
    {
        ContentPageService = contentPageService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        string cultureCode = CultureInfo.CurrentUICulture.Name;
        
        var model = new TopNavigationViewModel
        {
            NavigationPages = await ContentPageService.GetTopNavigationAsync(cultureCode),
            HomePagePages = await ContentPageService.GetHomePageNavigationAsync(cultureCode)
        };
        
        return View(model);
    }
}
