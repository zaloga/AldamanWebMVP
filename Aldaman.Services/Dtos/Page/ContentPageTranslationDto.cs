namespace Aldaman.Services.Dtos.Page;

public class ContentPageTranslationDto
{
    public string CultureCode { get; set; } = string.Empty;
    public string? Title { get; set; }
    public bool DisplayTitle { get; set; } = true;
    public string? Slug { get; set; }
    public string? BodyHtml { get; set; }
    public string? BodyDeltaJson { get; set; }
    public string? PlainText { get; set; }
}
