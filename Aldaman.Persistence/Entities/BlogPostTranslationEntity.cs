namespace Aldaman.Persistence.Entities;

public class BlogPostTranslationEntity : BaseEntity
{

    public Guid BlogPostId { get; set; }

    public string CultureCode { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Perex { get; set; }

    public string? BodyHtml { get; set; }

    public string? SearchText { get; set; }

    public string? SeoTitle { get; set; }

    public string? SeoDescription { get; set; }

    // Navigation property
    public virtual BlogPostEntity BlogPost { get; set; } = null!;
}
