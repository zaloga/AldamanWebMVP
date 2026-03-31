namespace Aldaman.Persistence.Entities;

/// <summary>
/// Obsah konkrétní stránky pro konkrétní jazyk.
/// </summary>
public class PageContentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PageDefinitionId { get; set; }

    /// <summary>
    /// Jazykový kód (např. cs, en).
    /// </summary>
    public string LanguageCode { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? SeoTitle { get; set; }

    public string? SeoDescription { get; set; }

    /// <summary>
    /// Serializovaný payload sekcí stránky.
    /// </summary>
    public string SectionsJson { get; set; } = "[]";

    public bool IsPublished { get; set; }

    public DateTime? PublishedAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    // Navigation properties
    public virtual PageDefinitionEntity PageDefinition { get; set; } = null!;
}
