using Aldaman.Persistence.Enums;

namespace Aldaman.Services.Dtos.ContentGroup;

/// <summary>
/// Detailed DTO for displaying a content group and its ordered items on the public web.
/// </summary>
public class ContentGroupDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public PlaceToShowEnum PlaceToShow { get; set; }
    public int GroupOrder { get; set; }
    public List<ContentGroupDetailItemDto> Items { get; set; } = new();
}
