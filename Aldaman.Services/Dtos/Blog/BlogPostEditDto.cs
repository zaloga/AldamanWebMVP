using System;
using System.Collections.Generic;

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
