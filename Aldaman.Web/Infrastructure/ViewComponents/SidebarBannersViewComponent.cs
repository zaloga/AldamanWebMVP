using System.Globalization;
using Aldaman.Services.Interfaces;
using Aldaman.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aldaman.Web.Infrastructure.ViewComponents;

public class SidebarBannersViewComponent : ViewComponent
{
    private IContentService ContentService { get; }

    public SidebarBannersViewComponent(IContentService contentService)
    {
        ContentService = contentService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        string cultureCode = CultureInfo.CurrentUICulture.Name;

        var banners = await ContentService.GetSidebarBannersCachedAsync(cultureCode);

        var model = new SidebarBannersViewModel
        {
            Banners = banners
        };

        return View(model);
    }
}
