using Aldaman.Services.Dtos.Navigation;

namespace Aldaman.Web.ViewModels;

public class TopNavigationViewModel
{
    public IEnumerable<NavigationDto> NavigationPages { get; set; } = new List<NavigationDto>();
}
