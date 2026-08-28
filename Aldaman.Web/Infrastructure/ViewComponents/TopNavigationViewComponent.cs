using System.Globalization;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Aldaman.Web.ViewModels;

namespace Aldaman.Web.Infrastructure.ViewComponents;

public class TopNavigationViewComponent : ViewComponent
{
    private INavigationService NavigationService { get; }

    public TopNavigationViewComponent(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        string cultureCode = CultureInfo.CurrentUICulture.Name;
        
        var model = new TopNavigationViewModel
        {
            NavigationPages = await NavigationService.GetTopNavigationAsync(cultureCode)
        };
        
        return View(model);
    }
}
