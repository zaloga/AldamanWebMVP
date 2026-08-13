using Aldaman.Services.Configuration;
using Microsoft.Extensions.Options;
using SkiaSharp;

namespace Aldaman.Services.Services.Images;

internal sealed class SkiaImageProcessingService : IImageProcessingService
{
    private IOptions<ImageProcessingSettings> SettingsOptions { get; }

    public SkiaImageProcessingService(IOptions<ImageProcessingSettings> settingsOptions)
    {
        SettingsOptions = settingsOptions;
    }

    public Task<byte[]> ProcessImageAsync(Stream inputStream, int targetWidth, int? targetHeight = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(inputStream);

        if (targetWidth <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(targetWidth), "Target width must be greater than zero.");
        }

        if (targetHeight.HasValue && targetHeight.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(targetHeight), "Target height must be greater than zero when specified.");
        }

        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            using SKBitmap originalBitmap = SKBitmap.Decode(inputStream);
            if (originalBitmap == null)
            {
                throw new InvalidOperationException("Failed to decode the provided image stream.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            int effectiveHeight = targetHeight.HasValue && targetHeight.Value > 0
                ? targetHeight.Value
                : (int)Math.Max(1, Math.Round((double)originalBitmap.Height * targetWidth / originalBitmap.Width));

            SKImageInfo imageInfo = new(targetWidth, effectiveHeight, originalBitmap.ColorType, originalBitmap.AlphaType, originalBitmap.ColorSpace);
            using SKBitmap resizedBitmap = originalBitmap.Resize(imageInfo, new SKSamplingOptions(SKCubicResampler.Mitchell));
            if (resizedBitmap == null)
            {
                throw new InvalidOperationException("Failed to resize image to target dimensions.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            using SKImage image = SKImage.FromBitmap(resizedBitmap);
            int quality = SettingsOptions.Value.Quality;
            if (quality is < 1 or > 100)
            {
                quality = 80;
            }

            using SKData encodedData = image.Encode(SKEncodedImageFormat.Webp, quality);
            if (encodedData == null)
            {
                throw new InvalidOperationException("Failed to encode image to WebP format.");
            }

            return encodedData.ToArray();
        }, cancellationToken);
    }
}
