using System.Globalization;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        var navigationPages = await ContentPageService.GetTopNavigationAsync(cultureCode);
        
        return View(navigationPages);
    }
}
