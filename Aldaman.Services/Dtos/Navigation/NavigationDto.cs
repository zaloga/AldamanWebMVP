namespace Aldaman.Services.Dtos.Navigation;

/// <summary>
/// DTO for rendering navigation links.
/// </summary>
public class NavigationDto
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsGroup { get; set; }
    public int Order { get; set; }
}
