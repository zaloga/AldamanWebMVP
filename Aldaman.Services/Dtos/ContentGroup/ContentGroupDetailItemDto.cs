using Aldaman.Services.Dtos.Content;

namespace Aldaman.Services.Dtos.ContentGroup;

/// <summary>
/// DTO representing an individual content item inside a ContentGroup.
/// </summary>
public class ContentGroupDetailItemDto
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public ContentDetailDto Content { get; set; } = default!;
}
