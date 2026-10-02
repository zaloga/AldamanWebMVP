using Aldaman.Persistence.Enums;
using Aldaman.Services.Dtos.Media;

namespace Aldaman.Services.Interfaces;

/// <summary>
/// Interface for physical or cloud storage provider of media files.
/// </summary>
public interface IFileStorageService
{
    StorageProviderType ProviderType { get; }

    Task<FileStorageResultDto> SaveAsync(Stream fileStream, string originalFileName, string contentType, CancellationToken ct = default);

    Task DeleteAsync(string storedFileName, CancellationToken ct = default);
}
