namespace Aldaman.Services.Dtos.Page;

/// <summary>
/// DTO for creating or updating a page with its localized content.
/// </summary>
public class PageEditDto
{
    public Guid? Id { get; set; }
    public string PageKey { get; set; } = string.Empty;
    public bool ShowOnHomePage { get; set; } = true;
    public int PageOrder { get; set; } = 0;
    public int DefaultSortOrder { get; set; }

    // Content specific properties for a single culture
    public string CultureCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public List<PageContentDto> Contents { get; set; } = new();
}
