namespace Aldaman.Persistence.Entities;

/// <summary>
/// Catalog of system pages.
/// </summary>
public class PageDefinitionEntity : BaseEntity
{
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
    /// Default sort order.
    /// </summary>
    public int DefaultSortOrder { get; set; }

    // Navigation properties
    public virtual ICollection<PageContentEntity> Contents { get; set; } = new List<PageContentEntity>();
}
