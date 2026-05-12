using Aldaman.Services.Dtos.Page;

namespace Aldaman.Web.ViewModels;

public class TopNavigationViewModel
{
    public IEnumerable<ContentPageNavigationDto> NavigationPages { get; set; } = new List<ContentPageNavigationDto>();
}
