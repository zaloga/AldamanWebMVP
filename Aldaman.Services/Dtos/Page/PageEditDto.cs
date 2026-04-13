namespace Aldaman.Services.Dtos.Page;

/// <summary>
/// DTO for creating or updating a page with its localized content.
/// </summary>
public class PageEditDto
{
    public Guid? Id { get; set; }
    public string PageKey { get; set; } = string.Empty;
    public bool IsHomePage { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public int DefaultSortOrder { get; set; }

    // Content specific properties for a single culture
    public string CultureCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public string? SeoKeywords { get; set; }
    public List<PageSectionDto> Sections { get; set; } = new();
    public bool IsPublished { get; set; }
    public List<PageContentDto> Contents { get; set; } = new();
}
