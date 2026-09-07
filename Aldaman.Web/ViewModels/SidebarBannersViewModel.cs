using Aldaman.Services.Dtos.Content;

namespace Aldaman.Web.ViewModels;

public class SidebarBannersViewModel
{
    public IEnumerable<ContentDetailDto> Banners { get; set; } = Enumerable.Empty<ContentDetailDto>();
}
