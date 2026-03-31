namespace Aldaman.Services.Dtos;

/// <summary>
/// Reference to a media file with its metadata.
/// </summary>
public class MediaAssetDto
{
    public Guid Id { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string RelativePath { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public string? Title { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
}
