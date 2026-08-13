namespace Aldaman.Services.Configuration;

/// <summary>
/// Settings for server-side image processing.
/// </summary>
public sealed class ImageProcessingSettings
{
    public const string SectionName = "ImageProcessing";

    /// <summary>
    /// Compression quality for WebP encoding (1 to 100). Default is 80.
    /// </summary>
    public int Quality { get; set; } = 80;
}
