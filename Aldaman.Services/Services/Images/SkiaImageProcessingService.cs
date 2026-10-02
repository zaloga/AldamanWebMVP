using Aldaman.Services.Configuration;
using Aldaman.Services.Dtos.Media;
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

    public Task<ProcessedImageResultDto> ProcessImageAsync(Stream inputStream, int? targetWidth = null, int? targetHeight = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(inputStream);

        if (targetWidth.HasValue && targetWidth.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(targetWidth), "Target width must be greater than zero when specified.");
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

            int effectiveWidth = targetWidth.HasValue && targetWidth.Value > 0
                ? targetWidth.Value
                : originalBitmap.Width;

            int effectiveHeight = targetHeight.HasValue && targetHeight.Value > 0
                ? targetHeight.Value
                : (targetWidth.HasValue && targetWidth.Value > 0
                    ? (int)Math.Max(1, Math.Round((double)originalBitmap.Height * targetWidth.Value / originalBitmap.Width))
                    : originalBitmap.Height);

            SKBitmap? resizedBitmap = null;
            SKImage image;

            try
            {
                if (effectiveWidth != originalBitmap.Width || effectiveHeight != originalBitmap.Height)
                {
                    SKImageInfo imageInfo = new(effectiveWidth, effectiveHeight, originalBitmap.ColorType, originalBitmap.AlphaType, originalBitmap.ColorSpace);
                    resizedBitmap = originalBitmap.Resize(imageInfo, new SKSamplingOptions(SKCubicResampler.Mitchell));
                    if (resizedBitmap == null)
                    {
                        throw new InvalidOperationException("Failed to resize image to target dimensions.");
                    }
                    image = SKImage.FromBitmap(resizedBitmap);
                }
                else
                {
                    image = SKImage.FromBitmap(originalBitmap);
                }

                using (image)
                {
                    cancellationToken.ThrowIfCancellationRequested();

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

                    return new ProcessedImageResultDto(encodedData.ToArray(), effectiveWidth, effectiveHeight);
                }
            }
            finally
            {
                resizedBitmap?.Dispose();
            }
        }, cancellationToken);
    }
}

