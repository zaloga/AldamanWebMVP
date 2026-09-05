using Aldaman.Persistence.Enums;

namespace Aldaman.Services.Dtos.Content;

/// <summary>
/// Detailed data for rendering a content item (page or post).
/// </summary>
public class ContentDetailDto
{
    public Guid Id { get; set; }
    public ContentTypeEnum ContentType { get; set; } = ContentTypeEnum.Post;
    public string Title { get; set; } = string.Empty;
    public bool DisplayTitle { get; set; } = true;
    public bool DisplayExpanded { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string? Perex { get; set; }
    public string? BodyHtml { get; set; }
    public string? BodyDeltaJson { get; set; }
    public string? PlainText { get; set; }
    public string? CoverImageRelativePath { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public bool IsPublished { get; set; }
    public PlaceToShowEnum PlaceToShow { get; set; }
    public int Order { get; set; }
    public string? AuthorName { get; set; }
}
