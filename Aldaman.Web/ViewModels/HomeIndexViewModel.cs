using Aldaman.Services.Dtos.Blog;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Dtos.Page;

namespace Aldaman.Web.ViewModels;

public class HomeIndexViewModel
{
    public IEnumerable<ContentPageDetailDto> HomePages { get; init; } = Enumerable.Empty<ContentPageDetailDto>();
    public PagedResultDto<BlogPostListItemDto> Posts { get; init; } = default!;
}
