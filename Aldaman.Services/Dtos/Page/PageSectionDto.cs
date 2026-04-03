namespace Aldaman.Services.Dtos.Page;

/// <summary>
/// Represents a single content section of a page.
/// </summary>
public class PageSectionDto
{
    public string Type { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = "{}";
}
