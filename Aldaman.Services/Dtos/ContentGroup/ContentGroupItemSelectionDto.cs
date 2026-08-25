using Aldaman.Persistence.Enums;

namespace Aldaman.Services.Dtos.ContentGroup;

/// <summary>
/// Selected item in a content group with its display order and type.
/// </summary>
public class ContentGroupItemSelectionDto
{
    public Guid Id { get; set; }
    public ContentGroupItemTypeEnum Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int Order { get; set; }
}
