namespace Aldaman.Services.Dtos.ContentGroup;

/// <summary>
/// Option for item dropdown selectors (Contents).
/// </summary>
public class ContentGroupItemOptionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}
