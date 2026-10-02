using Aldaman.Persistence.Enums;

namespace Aldaman.Services.Dtos.Media;

/// <summary>
/// Result returned after saving a file into physical/cloud storage.
/// </summary>
public sealed class FileStorageResultDto
{
    public string StoredFileName { get; set; } = string.Empty;

    public string RelativePath { get; set; } = string.Empty;

    public StorageProviderType StorageProvider { get; set; }
}
