namespace Aldaman.Services.Dtos.Search;

public sealed class SearchResultDto
{
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty; // "BlogPost" or "ContentPage"
}
