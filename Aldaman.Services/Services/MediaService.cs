using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Aldaman.Persistence.Enums;
using Aldaman.Services.Constants;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Dtos.Media;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace Aldaman.Services.Services;

public sealed class MediaService : IMediaService
{
    private AppDbContext Context { get; }
    private IFileStorageService StorageService { get; }
    private IServiceProvider ServiceProvider { get; }
    private ILogger<MediaService> Logger { get; }

    public MediaService(
        AppDbContext context,
        IFileStorageService storageService,
        IServiceProvider serviceProvider,
        ILogger<MediaService> logger)
    {
        Context = context;
        StorageService = storageService;
        ServiceProvider = serviceProvider;
        Logger = logger;
    }

    public async Task<PagedResultDto<MediaAssetDto>> ListAssetsAsync(PaginationQuery query, bool filterDeleted = false, bool onlyImages = false, CancellationToken ct = default)
    {
        IQueryable<MediaAssetEntity> dbQuery = filterDeleted
            ? Context.MediaAssets.IgnoreQueryFilters().Where(p => p.IsDeleted)
            : Context.MediaAssets.AsQueryable();

        if (onlyImages)
        {
            dbQuery = dbQuery.Where(p => p.IsImage);
        }

        // Filtering
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            dbQuery = dbQuery.Where(p => p.OriginalFileName.Contains(query.SearchTerm) || (p.AltTextDefault != null && p.AltTextDefault.Contains(query.SearchTerm)) || (p.TitleDefault != null && p.TitleDefault.Contains(query.SearchTerm)));
        }

        // Sorting
        dbQuery = query.SortBy switch
        {
            SortByConstants.FileName => query.SortDescending ? dbQuery.OrderByDescending(p => p.OriginalFileName) : dbQuery.OrderBy(p => p.OriginalFileName),
            SortByConstants.UploadedAt => query.SortDescending ? dbQuery.OrderByDescending(p => p.CreatedAtUtc) : dbQuery.OrderBy(p => p.CreatedAtUtc),
            SortByConstants.CreatedAt => query.SortDescending ? dbQuery.OrderByDescending(p => p.CreatedAtUtc) : dbQuery.OrderBy(p => p.CreatedAtUtc),
            SortByConstants.Size => query.SortDescending ? dbQuery.OrderByDescending(p => p.FileSize) : dbQuery.OrderBy(p => p.FileSize),
            SortByConstants.DeletedAt => query.SortDescending ? dbQuery.OrderByDescending(p => p.DeletedAtUtc) : dbQuery.OrderBy(p => p.DeletedAtUtc),
            _ => filterDeleted
                ? dbQuery.OrderByDescending(p => p.DeletedAtUtc)
                : dbQuery.OrderByDescending(p => p.CreatedAtUtc)
        };

        int totalCount = await dbQuery.CountAsync(ct);
        List<MediaAssetDto> items = await dbQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => Map(p))
            .ToListAsync(ct);

        return new PagedResultDto<MediaAssetDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<MediaAssetDto> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken ct = default)
    {
        if (fileStream.CanSeek && fileStream.Length > 1 * 1024 * 1024)
        {
            throw new InvalidOperationException("File size exceeds the 1 MB limit.");
        }

        using MemoryStream bufferStream = new();
        if (fileStream.CanSeek)
        {
            fileStream.Position = 0;
        }
        await fileStream.CopyToAsync(bufferStream, ct);

        if (bufferStream.Length > 1 * 1024 * 1024)
        {
            throw new InvalidOperationException("File size exceeds the 1 MB limit.");
        }

        bufferStream.Position = 0;

        bool isImage = contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        int? width = null;
        int? height = null;

        if (isImage)
        {
            try
            {
                byte[] fileBytes = bufferStream.ToArray();
                using SKData data = SKData.CreateCopy(fileBytes);
                using SKCodec codec = SKCodec.Create(data);
                if (codec != null)
                {
                    width = codec.Info.Width;
                    height = codec.Info.Height;
                }
                else
                {
                    using SKBitmap bitmap = SKBitmap.Decode(data);
                    if (bitmap != null)
                    {
                        width = bitmap.Width;
                        height = bitmap.Height;
                    }
                    else
                    {
                        Logger.LogWarning("Failed to decode image file {FileName} using SkiaSharp. Treating as non-image.", fileName);
                        isImage = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error reading image dimensions for file {FileName}.", fileName);
                isImage = false;
            }

            bufferStream.Position = 0;
        }

        long fileSize = bufferStream.Length;
        FileStorageResultDto storageResult = await StorageService.SaveAsync(bufferStream, fileName, contentType, ct);

        MediaAssetEntity asset = new()
        {
            OriginalFileName = fileName,
            StoredFileName = storageResult.StoredFileName,
            RelativePath = storageResult.RelativePath,
            ContentType = contentType,
            FileSize = fileSize,
            IsImage = isImage,
            IsVideo = contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase),
            Width = width,
            Height = height,
            StorageProvider = storageResult.StorageProvider
        };

        Context.MediaAssets.Add(asset);
        await Context.SaveChangesAsync(ct);

        return Map(asset);
    }

    public async Task<MediaAssetDto?> GetAssetAsync(Guid id, CancellationToken ct = default)
    {
        MediaAssetEntity? asset = await Context.MediaAssets
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        return asset != null ? Map(asset) : null;
    }

    public async Task UpdateAssetAsync(UpdateMediaAssetDto dto, CancellationToken ct = default)
    {
        MediaAssetEntity? asset = await Context.MediaAssets.FirstOrDefaultAsync(p => p.Id == dto.Id, ct);
        if (asset != null)
        {
            asset.AltTextDefault = dto.AltTextDefault;
            asset.TitleDefault = dto.TitleDefault;
            asset.UpdatedAtUtc = DateTime.UtcNow;

            await Context.SaveChangesAsync(ct);
        }
    }

    public async Task DeleteAssetAsync(Guid id, CancellationToken ct = default)
    {
        MediaAssetEntity? asset = await Context.MediaAssets.FindAsync([id], cancellationToken: ct);
        if (asset != null)
        {
            asset.IsDeleted = true;
            asset.DeletedAtUtc = DateTime.UtcNow;
            await Context.SaveChangesAsync(ct);
        }
    }

    public async Task RestoreAssetAsync(Guid id, CancellationToken ct = default)
    {
        MediaAssetEntity? asset = await Context.MediaAssets
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (asset != null)
        {
            asset.IsDeleted = false;
            asset.DeletedAtUtc = null;
            await Context.SaveChangesAsync(ct);
        }
    }

    public async Task HardDeleteAssetAsync(Guid id, CancellationToken ct = default)
    {
        MediaAssetEntity? asset = await Context.MediaAssets
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (asset != null)
        {
            IFileStorageService targetStorage = ServiceProvider.GetKeyedService<IFileStorageService>(asset.StorageProvider)
                ?? ServiceProvider.GetRequiredKeyedService<IFileStorageService>(StorageProviderType.FileSystem);

            await targetStorage.DeleteAsync(asset.StoredFileName, ct);

            Context.MediaAssets.Remove(asset);
            await Context.SaveChangesAsync(ct);
        }
    }

    public async Task DeleteMediaAsync(IEnumerable<string> relativePaths, CancellationToken ct = default)
    {
        foreach (string path in relativePaths.Distinct())
        {
            MediaAssetEntity? asset = await Context.MediaAssets
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(a => a.RelativePath == path, ct);
            if (asset != null)
            {
                await HardDeleteAssetAsync(asset.Id, ct);
            }
        }
    }

    private static MediaAssetDto Map(MediaAssetEntity p)
    {
        return new MediaAssetDto
        {
            Id = p.Id,
            OriginalFileName = p.OriginalFileName,
            RelativePath = p.RelativePath,
            ContentType = p.ContentType,
            FileSize = p.FileSize,
            Width = p.Width,
            Height = p.Height,
            StorageProvider = p.StorageProvider,
            AltTextDefault = p.AltTextDefault,
            TitleDefault = p.TitleDefault,
            UploadedAtUtc = p.CreatedAtUtc,
            UpdatedAtUtc = p.UpdatedAtUtc,
            IsImage = p.IsImage,
            IsVideo = p.IsVideo,
            IsDeleted = p.IsDeleted,
            DeletedAtUtc = p.DeletedAtUtc
        };
    }
}
