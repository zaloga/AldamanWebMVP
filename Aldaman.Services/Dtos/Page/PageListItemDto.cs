using System;

namespace Aldaman.Services.Dtos.Page;

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
    public int DefaultSortOrder { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
