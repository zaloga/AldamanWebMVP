namespace Aldaman.Persistence.Entities;

/// <summary>
/// Content of a specific page for a specific language.
/// </summary>
public class PageContentEntity : BaseEntity
{
    public const int CultureCodeMaxLength = 5;
    public const int TitleMaxLength = 256;
    public const int SlugMaxLength = 256;

    public Guid PageDefinitionId { get; set; }

    /// <summary>
    /// Culture code (e.g. cs-CZ, en-US).
    /// </summary>
    public string CultureCode { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public bool IsPublished { get; set; }

    public DateTime? PublishedAtUtc { get; set; }

    // Navigation properties
    public virtual PageDefinitionEntity PageDefinition { get; set; } = null!;
}
