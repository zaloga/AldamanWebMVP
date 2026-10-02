using Aldaman.Persistence.Enums;
using Aldaman.Services.Configuration;
using Aldaman.Services.Dtos.Media;
using Aldaman.Services.Interfaces;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aldaman.Services.Services;

public sealed class CloudflareR2StorageService : IFileStorageService
{
    private IOptions<MediaStorageSettings> SettingsOptions { get; }
    private ILogger<CloudflareR2StorageService> Logger { get; }

    public StorageProviderType ProviderType => StorageProviderType.CloudflareR2;

    public CloudflareR2StorageService(
        IOptions<MediaStorageSettings> settingsOptions,
        ILogger<CloudflareR2StorageService> logger)
    {
        SettingsOptions = settingsOptions;
        Logger = logger;
    }

    private AmazonS3Client CreateClient()
    {
        CloudflareR2Settings settings = SettingsOptions.Value.CloudflareR2;
        BasicAWSCredentials credentials = new(settings.AccessKey, settings.SecretKey);
        AmazonS3Config config = new()
        {
            ServiceURL = settings.ServiceUrl,
            ForcePathStyle = true
        };

        return new AmazonS3Client(credentials, config);
    }

    public async Task<FileStorageResultDto> SaveAsync(Stream fileStream, string originalFileName, string contentType, CancellationToken ct = default)
    {
        CloudflareR2Settings settings = SettingsOptions.Value.CloudflareR2;

        if (string.IsNullOrWhiteSpace(settings.BucketName))
        {
            throw new InvalidOperationException("Cloudflare R2 BucketName is not configured.");
        }

        string extension = Path.GetExtension(originalFileName);
        string storedFileName = $"{Guid.NewGuid()}{extension}";

        using AmazonS3Client client = CreateClient();

        PutObjectRequest putRequest = new()
        {
            BucketName = settings.BucketName,
            Key = storedFileName,
            InputStream = fileStream,
            ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType,
            DisablePayloadSigning = true,
            AutoCloseStream = false
        };

        await client.PutObjectAsync(putRequest, ct);

        string publicUrlBase = settings.PublicUrlBase.TrimEnd('/');
        string bucketSegment = publicUrlBase.EndsWith($"/{settings.BucketName}", StringComparison.OrdinalIgnoreCase)
            ? string.Empty
            : $"/{settings.BucketName}";
        string relativePath = $"{publicUrlBase}{bucketSegment}/{storedFileName}";

        return new FileStorageResultDto
        {
            StoredFileName = storedFileName,
            RelativePath = relativePath,
            StorageProvider = StorageProviderType.CloudflareR2
        };
    }

    public async Task DeleteAsync(string storedFileName, CancellationToken ct = default)
    {
        CloudflareR2Settings settings = SettingsOptions.Value.CloudflareR2;

        if (string.IsNullOrWhiteSpace(settings.BucketName))
        {
            Logger.LogWarning("Cannot delete R2 file {StoredFileName}: BucketName is empty.", storedFileName);
            return;
        }

        try
        {
            using AmazonS3Client client = CreateClient();
            DeleteObjectRequest deleteRequest = new()
            {
                BucketName = settings.BucketName,
                Key = storedFileName
            };

            await client.DeleteObjectAsync(deleteRequest, ct);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to delete object {StoredFileName} from Cloudflare R2 bucket {BucketName}.", storedFileName, settings.BucketName);
        }
    }
}
