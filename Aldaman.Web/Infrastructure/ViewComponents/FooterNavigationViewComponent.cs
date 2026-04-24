using System.Globalization;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Infrastructure.ViewComponents;

public class FooterNavigationViewComponent : ViewComponent
{
    private IContentPageService ContentPageService { get; }

    public FooterNavigationViewComponent(IContentPageService contentPageService)
    {
        ContentPageService = contentPageService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        string cultureCode = CultureInfo.CurrentUICulture.Name;
        var footerPages = await ContentPageService.GetFooterNavigationAsync(cultureCode);
        
        return View(footerPages);
    }
}
