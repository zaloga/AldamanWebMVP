using Aldaman.Services.Dtos.Media;

namespace Aldaman.Services.Services.Images;

/// <summary>
/// Service interface for server-side image decoding, resizing, and re-encoding using SkiaSharp.
/// </summary>
public interface IImageProcessingService
{
    /// <summary>
    /// Resizes or optimizes an input image stream and encodes it as WebP format.
    /// If target dimensions are not provided, original dimensions are preserved.
    /// </summary>
    /// <param name="inputStream">Stream containing original image binary data.</param>
    /// <param name="targetWidth">Optional target width in pixels.</param>
    /// <param name="targetHeight">Optional target height in pixels.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Processed WebP image result containing byte array and effective dimensions.</returns>
    Task<ProcessedImageResultDto> ProcessImageAsync(Stream inputStream, int? targetWidth = null, int? targetHeight = null, CancellationToken cancellationToken = default);
}

