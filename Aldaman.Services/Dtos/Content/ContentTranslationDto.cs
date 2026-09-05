namespace Aldaman.Services.Dtos.Content;

public class ContentTranslationDto
{
    public string CultureCode { get; set; } = string.Empty;
    public string? Title { get; set; }
    public bool DisplayTitle { get; set; } = true;
    public string? Slug { get; set; }
    public string? Perex { get; set; }
    public string? BodyHtml { get; set; }
    public string? BodyDeltaJson { get; set; }
    public string? PlainText { get; set; }
    public bool DisplayExpanded { get; set; } = false;
}
