using Aldaman.Services.Dtos.Media;

namespace Aldaman.Services.Dtos.Blog;

/// <summary>
/// Detailed data for public blog post page.
/// </summary>
public class BlogPostDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Perex { get; set; }
    public string? BodyHtml { get; set; }
    public string? PlainText { get; set; }
    public MediaAssetDto? CoverImage { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public string? AuthorName { get; set; }
    public string? CoverImageUrl { get; set; }

}
