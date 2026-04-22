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
    public int OrderOnHomePage { get; set; }
    public int OrderInNavigation { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
