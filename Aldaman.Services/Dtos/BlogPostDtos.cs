namespace Aldaman.Services.Dtos;

/// <summary>
/// Minimal data for blog post lists (home, category, etc.).
/// </summary>
public class BlogPostListItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Perex { get; set; }
    public MediaAssetDto? CoverImage { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public bool IsPublished { get; set; }
    public string? AuthorName { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

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
    public MediaAssetDto? CoverImage { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public string? AuthorName { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
}

/// <summary>
/// DTO for creating or updating a blog post with its localized content.
/// </summary>
public class BlogPostEditDto
{
    public Guid? Id { get; set; }
    public Guid? CoverMediaAssetId { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAtUtc { get; set; }

    // Content specific properties for a single culture
    public string CultureCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Perex { get; set; }
    public string? BodyHtml { get; set; }
    public string? SearchText { get; set; }
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    
    // Support for multiple translations in admin
    public List<BlogPostTranslationDto> Translations { get; set; } = new();
}

public class BlogPostTranslationDto
{
    public string CultureCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Perex { get; set; }
    public string? BodyHtml { get; set; }
}
