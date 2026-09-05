using Aldaman.Services.Dtos.Content;

namespace Aldaman.Web.ViewModels;

public class ContentViewModel
{
    public ContentDetailDto Content { get; init; } = default!;
    public ContentNavigationDto? PreviousContent { get; init; }
    public ContentNavigationDto? NextContent { get; init; }
}
