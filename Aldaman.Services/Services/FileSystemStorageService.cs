using Aldaman.Persistence.Enums;
using Aldaman.Services.Dtos.Media;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Aldaman.Services.Services;

public sealed class FileSystemStorageService : IFileStorageService
{
    private const string UploadsFolder = "uploads";

    private string WebRootPath { get; }
    private ILogger<FileSystemStorageService> Logger { get; }

    public StorageProviderType ProviderType => StorageProviderType.FileSystem;

    public FileSystemStorageService(IWebHostEnvironment environment, ILogger<FileSystemStorageService> logger)
    {
        ArgumentNullException.ThrowIfNull(environment);

        if (string.IsNullOrWhiteSpace(environment.WebRootPath))
        {
            throw new InvalidOperationException("WebRootPath is not configured on IWebHostEnvironment.");
        }

        WebRootPath = environment.WebRootPath;
        Logger = logger;
    }

    public async Task<FileStorageResultDto> SaveAsync(Stream fileStream, string originalFileName, string contentType, CancellationToken ct = default)
    {
        string uploadsFolderPath = Path.Combine(WebRootPath, UploadsFolder);
        if (!Directory.Exists(uploadsFolderPath))
        {
            Directory.CreateDirectory(uploadsFolderPath);
        }

        string extension = Path.GetExtension(originalFileName);
        string storedFileName = $"{Guid.NewGuid()}{extension}";
        string physicalPath = Path.Combine(uploadsFolderPath, storedFileName);
        string relativePath = $"/{UploadsFolder}/{storedFileName}";

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
        string uploadsFolderPath = Path.Combine(WebRootPath, UploadsFolder);
        string physicalPath = Path.Combine(uploadsFolderPath, storedFileName);

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
