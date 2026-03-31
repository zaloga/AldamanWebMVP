namespace Aldaman.Persistence.Entities;

/// <summary>
/// Catalog of system pages.
/// </summary>
public class PageDefinitionEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Unique page key (e.g. home, about).
    /// </summary>
    public string PageKey { get; set; } = string.Empty;

    /// <summary>
    /// URL route segment.
    /// </summary>
    public string RouteSegment { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether this is the home page.
    /// </summary>
    public bool IsHomePage { get; set; }

    /// <summary>
    /// Indicates whether the page is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Default sort order.
    /// </summary>
    public int DefaultSortOrder { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    // Navigation properties
    public virtual ICollection<PageContentEntity> Contents { get; set; } = new List<PageContentEntity>();
}
