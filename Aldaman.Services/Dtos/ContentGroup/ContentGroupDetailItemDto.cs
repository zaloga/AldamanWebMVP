using Aldaman.Persistence.Enums;
using Aldaman.Services.Dtos.Blog;
using Aldaman.Services.Dtos.Page;

namespace Aldaman.Services.Dtos.ContentGroup;

/// <summary>
/// DTO representing an individual item (ContentPage or BlogPost) inside a ContentGroup.
/// </summary>
public class ContentGroupDetailItemDto
{
    public Guid Id { get; set; }
    public ContentGroupItemTypeEnum Type { get; set; }
    public int Order { get; set; }
    public ContentPageDetailDto? Page { get; set; }
    public BlogPostListItemDto? BlogPost { get; set; }
}
