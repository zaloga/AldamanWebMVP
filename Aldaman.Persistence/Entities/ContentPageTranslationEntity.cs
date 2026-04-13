namespace Aldaman.Persistence.Entities;

/// <summary>
/// Content of a specific page for a specific language.
/// </summary>
public class ContentPageTranslationEntity : BaseEntity
{
    public const int CultureCodeMaxLength = 5;
    public const int TitleMaxLength = 256;
    public const int SlugMaxLength = 256;

    public Guid ContentPageId { get; set; }

    /// <summary>
    /// Culture code (e.g. cs-CZ, en-US).
    /// </summary>
    public string CultureCode { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    // Navigation properties
    public virtual ContentPageEntity ContentPage { get; set; } = null!;
}
