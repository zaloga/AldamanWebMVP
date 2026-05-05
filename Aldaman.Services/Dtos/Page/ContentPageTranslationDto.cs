namespace Aldaman.Services.Dtos.Page;

public class ContentPageTranslationDto
{
    public string CultureCode { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Slug { get; set; }
    public string? BodyHtml { get; set; }
    public string? BodyDeltaJson { get; set; }
    public string? PlainText { get; set; }
}
