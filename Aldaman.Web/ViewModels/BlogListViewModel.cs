using Aldaman.Services.Dtos.Blog;
using Aldaman.Services.Dtos.General;

namespace Aldaman.Web.ViewModels;

public class BlogListViewModel
{
    public PagedResultDto<BlogPostListItemDto> Posts { get; init; } = default!;
}
