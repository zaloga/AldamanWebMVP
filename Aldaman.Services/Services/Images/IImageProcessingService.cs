namespace Aldaman.Services.Services.Images;

/// <summary>
/// Service interface for server-side image decoding, resizing, and re-encoding using SkiaSharp.
/// </summary>
public interface IImageProcessingService
{
    /// <summary>
    /// Resizes an input image stream to the specified dimensions and encodes it as WebP format.
    /// </summary>
    /// <param name="inputStream">Stream containing original image binary data.</param>
    /// <param name="targetWidth">Target width in pixels.</param>
    /// <param name="targetHeight">Target height in pixels.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Processed WebP image byte array.</returns>
    Task<byte[]> ProcessImageAsync(Stream inputStream, int targetWidth, int targetHeight, CancellationToken cancellationToken = default);
}
