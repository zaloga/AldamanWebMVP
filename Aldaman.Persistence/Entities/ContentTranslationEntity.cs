namespace Aldaman.Persistence.Entities;

/// <summary>
/// Localized translation of a content entity.
/// </summary>
public class ContentTranslationEntity : BaseEntityAuditable
{
    public const int CultureCodeMaxLength = 5;
    public const int TitleMaxLength = 256;
    public const int SlugMaxLength = 256;
    public const int PerexMaxLength = 1024;
    public const int PlainTextMaxLength = 2048;

    public Guid ContentId { get; set; }

    /// <summary>
    /// Culture code (e.g. cs, en, cs-CZ, en-US).
    /// </summary>
    public string CultureCode { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the title should be displayed as a heading on the page.
    /// </summary>
    public bool DisplayTitle { get; set; } = true;

    public string Slug { get; set; } = string.Empty;

    public string? Perex { get; set; }

    public string? BodyHtml { get; set; }

    public string? BodyDeltaJson { get; set; }

    public string? PlainText { get; set; }

    /// <summary>
    /// Indicates whether content is initially rendered expanded in summary lists.
    /// </summary>
    public bool DisplayExpanded { get; set; } = false;

    // Navigation property
    public virtual ContentEntity Content { get; set; } = null!;
}
