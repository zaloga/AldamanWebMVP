using Aldaman.Persistence.Enums;

namespace Aldaman.Services.Configuration;

/// <summary>
/// Settings for media file storage provider selection and configurations.
/// </summary>
public sealed class MediaStorageSettings
{
    public const string SectionName = "MediaStorage";

    public StorageProviderType Provider { get; set; } = StorageProviderType.FileSystem;

    public CloudflareR2Settings CloudflareR2 { get; set; } = new();
}
