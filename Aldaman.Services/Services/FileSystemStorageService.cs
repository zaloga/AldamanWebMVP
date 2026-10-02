using Aldaman.Persistence.Enums;
using Aldaman.Services.Dtos.Media;
using Aldaman.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Aldaman.Services.Services;

public sealed class FileSystemStorageService : IFileStorageService
{
    private string WebRootPath { get; }
    private ILogger<FileSystemStorageService> Logger { get; }

    public StorageProviderType ProviderType => StorageProviderType.FileSystem;

    public FileSystemStorageService(string webRootPath, ILogger<FileSystemStorageService> logger)
    {
        WebRootPath = webRootPath;
        Logger = logger;
    }

    public async Task<FileStorageResultDto> SaveAsync(Stream fileStream, string originalFileName, string contentType, CancellationToken ct = default)
    {
        string uploadsFolder = Path.Combine(WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        string extension = Path.GetExtension(originalFileName);
        string storedFileName = $"{Guid.NewGuid()}{extension}";
        string physicalPath = Path.Combine(uploadsFolder, storedFileName);
        string relativePath = $"/uploads/{storedFileName}";

        await using (FileStream fs = new(physicalPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await fileStream.CopyToAsync(fs, ct);
        }

        return new FileStorageResultDto
        {
            StoredFileName = storedFileName,
            RelativePath = relativePath,
            StorageProvider = StorageProviderType.FileSystem
        };
    }

    public Task DeleteAsync(string storedFileName, CancellationToken ct = default)
    {
        string uploadsFolder = Path.Combine(WebRootPath, "uploads");
        string physicalPath = Path.Combine(uploadsFolder, storedFileName);

        try
        {
            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to delete physical file {PhysicalPath} from file system.", physicalPath);
        }

        return Task.CompletedTask;
    }
}
