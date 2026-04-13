using System;

namespace Aldaman.Services.Dtos.Page;

/// <summary>
/// Minimal page data for admin management lists.
/// </summary>
public class ContentPageListItemDto
{
    public Guid Id { get; set; }
    public string PageKey { get; set; } = string.Empty;
    public bool ShowOnHomePage { get; set; }
    public int PageOrder { get; set; }
    public int DefaultSortOrder { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
