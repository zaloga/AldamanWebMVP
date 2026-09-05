using Aldaman.Persistence.Enums;

namespace Aldaman.Services.Dtos.Content;

/// <summary>
/// Summary data for content lists (admin management, home/feed lists).
/// </summary>
public class ContentListItemDto
{
    public Guid Id { get; set; }
    public ContentTypeEnum ContentType { get; set; } = ContentTypeEnum.Post;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Perex { get; set; }
    public string? CoverImageRelativePath { get; set; }
    public bool DisplayExpanded { get; set; }
    public string? BodyHtml { get; set; }
    public PlaceToShowEnum PlaceToShow { get; set; }
    public int Order { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
}
