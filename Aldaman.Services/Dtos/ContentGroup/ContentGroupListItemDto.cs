using Aldaman.Persistence.Enums;

namespace Aldaman.Services.Dtos.ContentGroup;

/// <summary>
/// Minimal content group data for admin management lists.
/// </summary>
public class ContentGroupListItemDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public PlaceToShowEnum PlaceToShow { get; set; }
    public int GroupOrder { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
}
