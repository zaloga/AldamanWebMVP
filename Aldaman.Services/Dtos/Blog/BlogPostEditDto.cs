namespace Aldaman.Services.Dtos.Blog;

/// <summary>
/// DTO for creating or updating a blog post with its localized content.
/// </summary>
public class BlogPostEditDto
{
    public Guid? Id { get; set; }
    public Guid? CoverMediaAssetId { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAtUtc { get; set; }

    // Support for multiple translations in admin
    public List<BlogPostTranslationDto> Translations { get; set; } = new();
}
