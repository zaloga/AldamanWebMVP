using System.Globalization;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Infrastructure.ViewComponents;

public class BrandViewComponent : ViewComponent
{
    private INavigationService NavigationService { get; }

    public BrandViewComponent(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        string cultureCode = CultureInfo.CurrentUICulture.Name;
        var homePagePages = await NavigationService.GetHomePageNavigationAsync(cultureCode);
        
        return View(homePagePages);
    }
}
