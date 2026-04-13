namespace Aldaman.Persistence.Entities;

/// <summary>
/// Catalog of system pages.
/// </summary>
public class PageDefinitionEntity : BaseEntity
{
    public const int PageKeyMaxLength = 256;

    /// <summary>
    /// Unique page key (e.g. home, about).
    /// </summary>
    public string PageKey { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether this page should be displayed on the home page.
    /// </summary>
    public bool ShowOnHomePage { get; set; }

    /// <summary>
    /// Order of the page on the home page.
    /// </summary>
    public int PageOrder { get; set; }

    /// <summary>
    /// Default sort order.
    /// </summary>
    public int DefaultSortOrder { get; set; }

    // Navigation properties
    public virtual ICollection<PageContentEntity> Contents { get; set; } = new List<PageContentEntity>();
}
