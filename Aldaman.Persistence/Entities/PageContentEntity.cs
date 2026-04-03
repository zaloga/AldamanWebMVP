namespace Aldaman.Persistence.Entities;

/// <summary>
/// Content of a specific page for a specific language.
/// </summary>
public class PageContentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PageDefinitionId { get; set; }

    /// <summary>
    /// Culture code (e.g. cs-CZ, en-US).
    /// </summary>
    public string CultureCode { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? SeoTitle { get; set; }

    public string? SeoDescription { get; set; }
    
    public string? SeoKeywords { get; set; }

    /// <summary>
    /// Serialized payload of page sections.
    /// </summary>
    public string SectionsJson { get; set; } = "[]";

    public bool IsPublished { get; set; }

    public DateTime? PublishedAtUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; set; }

    // Navigation properties
    public virtual PageDefinitionEntity PageDefinition { get; set; } = null!;
}
