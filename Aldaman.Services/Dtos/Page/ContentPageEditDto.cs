namespace Aldaman.Services.Dtos.Page;

/// <summary>
/// DTO for creating or updating a page with its localized content.
/// </summary>
public class ContentPageEditDto
{
    public Guid? Id { get; set; }
    public string PageKey { get; set; } = string.Empty;
    public bool ShowOnHomePage { get; set; } = true;
    public int OrderOnHomePage { get; set; } = 0;
    public int OrderInNavigation { get; set; } = 0;

    public List<ContentPageTranslationDto> Translations { get; set; } = new();
}
