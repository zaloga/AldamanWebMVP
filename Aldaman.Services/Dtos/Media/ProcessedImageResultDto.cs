namespace Aldaman.Services.Dtos.Media;

/// <summary>
/// Result of an image processing operation.
/// </summary>
/// <param name="Data">Encoded image bytes.</param>
/// <param name="Width">Effective image width in pixels.</param>
/// <param name="Height">Effective image height in pixels.</param>
public sealed record ProcessedImageResultDto(byte[] Data, int Width, int Height);
