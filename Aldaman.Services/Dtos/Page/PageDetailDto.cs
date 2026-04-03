using System;
using System.Collections.Generic;

namespace Aldaman.Services.Dtos.Page;

/// <summary>
/// Detailed data for public page rendering.
/// </summary>
public class PageDetailDto
{
    public Guid Id { get; set; }
    public string PageKey { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public string? SeoKeywords { get; set; }
    public string? SectionsJson { get; set; }
    public List<PageSectionDto> Sections { get; set; } = new();
}
