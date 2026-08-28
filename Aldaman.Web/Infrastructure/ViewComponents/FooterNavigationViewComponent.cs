using System.Globalization;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Infrastructure.ViewComponents;

public class FooterNavigationViewComponent : ViewComponent
{
    private INavigationService NavigationService { get; }

    public FooterNavigationViewComponent(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        string cultureCode = CultureInfo.CurrentUICulture.Name;
        var footerPages = await NavigationService.GetFooterNavigationAsync(cultureCode);
        
        return View(footerPages);
    }
}
