using System;
using Aldaman.Services.Dtos.Media;

namespace Aldaman.Services.Dtos.Blog;

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
