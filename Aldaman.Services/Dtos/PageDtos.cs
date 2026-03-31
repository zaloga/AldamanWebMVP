namespace Aldaman.Services.Dtos;

/// <summary>
/// Minimal page data for admin management lists.
/// </summary>
public class PageListItemDto
{
    public Guid Id { get; set; }
    public string PageKey { get; set; } = string.Empty;
    public string RouteSegment { get; set; } = string.Empty;
    public bool IsHomePage { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

/// <summary>
/// Detailed data for public page rendering.
/// </summary>
public class PageDetailDto
{
    public string PageKey { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public List<PageSectionDto> Sections { get; set; } = new();
}

/// <summary>
/// Represents a single content section of a page.
/// </summary>
public class PageSectionDto
{
    public string Type { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = "{}";
}

/// <summary>
/// DTO for creating or updating a page with its localized content.
/// </summary>
public class PageEditDto
{
    public Guid? Id { get; set; }
    public string PageKey { get; set; } = string.Empty;
    public string RouteSegment { get; set; } = string.Empty;
    public bool IsHomePage { get; set; }
    public bool IsActive { get; set; } = true;
    public int DefaultSortOrder { get; set; }

    // Content specific properties for a single culture
    public string CultureCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public List<PageSectionDto> Sections { get; set; } = new();
    public bool IsPublished { get; set; }
}
