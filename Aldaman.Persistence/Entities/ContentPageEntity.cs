using Aldaman.Persistence.Enums;

namespace Aldaman.Persistence.Entities;

/// <summary>
/// Catalog of system pages.
/// </summary>
public class ContentPageEntity : BaseEntity
{
    public const int PageKeyMaxLength = 256;

    /// <summary>
    /// Unique page key (e.g. home, about).
    /// </summary>
    public string PageKey { get; set; } = string.Empty;

    /// <summary>
    /// Indicates where this page should be displayed.
    /// </summary>
    public PlaceToShowEnum PlaceToShow { get; set; }

    /// <summary>
    /// Sorting order of the page.
    /// </summary>
    public int PageOrder { get; set; }

    // Navigation properties
    public virtual ICollection<ContentPageTranslationEntity> Translations { get; set; } = new List<ContentPageTranslationEntity>();
}
