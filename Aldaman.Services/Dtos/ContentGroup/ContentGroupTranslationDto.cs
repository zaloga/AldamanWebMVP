namespace Aldaman.Services.Dtos.ContentGroup;

public class ContentGroupTranslationDto
{
    public string CultureCode { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Slug { get; set; }
}
