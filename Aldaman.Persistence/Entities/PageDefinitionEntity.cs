using System.ComponentModel.DataAnnotations;

namespace Aldaman.Persistence.Entities;

/// <summary>
/// Katalog systémových stránek.
/// </summary>
public class PageDefinitionEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Unikátní klíč stránky (např. home, about).
    /// </summary>
    public string PageKey { get; set; } = string.Empty;

    /// <summary>
    /// Část URL cesty.
    /// </summary>
    public string RouteSegment { get; set; } = string.Empty;

    /// <summary>
    /// Příznak, zda se jedná o úvodní stránku.
    /// </summary>
    public bool IsHomePage { get; set; }

    /// <summary>
    /// Příznak, zda je stránka aktivní.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Výchozí pořadí řazení.
    /// </summary>
    public int DefaultSortOrder { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    // Navigation properties
    public virtual ICollection<PageContentEntity> Contents { get; set; } = new List<PageContentEntity>();
}
