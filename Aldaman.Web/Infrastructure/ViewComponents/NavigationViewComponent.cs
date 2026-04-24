using System.Globalization;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Infrastructure.ViewComponents;

public class NavigationViewComponent : ViewComponent
{
    private IContentPageService ContentPageService { get; }

    public NavigationViewComponent(IContentPageService contentPageService)
    {
        ContentPageService = contentPageService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        string cultureCode = CultureInfo.CurrentUICulture.Name;
        var navigationPages = await ContentPageService.GetNavigationPagesAsync(cultureCode);
        
        return View(navigationPages);
    }
}
