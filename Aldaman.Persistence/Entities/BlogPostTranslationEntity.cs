namespace Aldaman.Persistence.Entities;

public class BlogPostTranslationEntity : BaseEntity
{
    public const int CultureCodeMaxLength = 5;
    public const int TitleMaxLength = 256;
    public const int SlugMaxLength = 256;
    public const int PerexMaxLength = 1024;
    public const int SeoTitleMaxLength = 256;
    public const int SeoDescriptionMaxLength = 512;
    public const int PlainTextMaxLength = 2048;


    public Guid BlogPostId { get; set; }

    public string CultureCode { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Perex { get; set; }

    public string? BodyHtml { get; set; }

    public string? PlainText { get; set; }

    public string? SeoTitle { get; set; }

    public string? SeoDescription { get; set; }

    // Navigation property
    public virtual BlogPostEntity BlogPost { get; set; } = null!;
}
