using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Aldaman.Services.Constants;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Dtos.Media;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace Aldaman.Services.Services;

public sealed class MediaService : IMediaService
{
    private AppDbContext Context { get; }
    private string WebRootPath { get; }
    private ILogger<MediaService> Logger { get; }

    public MediaService(AppDbContext context, string webRootPath, ILogger<MediaService> logger)
    {
        Context = context;
        WebRootPath = webRootPath;
        Logger = logger;
    }

    public async Task<PagedResultDto<MediaAssetDto>> ListAssetsAsync(PaginationQuery query, bool filterDeleted = false, bool onlyImages = false, CancellationToken ct = default)
    {
        var dbQuery = filterDeleted
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

        var totalCount = await dbQuery.CountAsync(ct);
        var items = await dbQuery
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
        if (fileStream.Length > 1 * 1024 * 1024)
        {
            throw new InvalidOperationException("File size exceeds the 1 MB limit.");
        }

        var uploadsFolder = Path.Combine(WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var extension = Path.GetExtension(fileName);
        var storedFileName = $"{Guid.NewGuid()}{extension}";
        var physicalPath = Path.Combine(uploadsFolder, storedFileName);
        var relativePath = $"/uploads/{storedFileName}";

        using (var fs = new FileStream(physicalPath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fs, ct);
        }

        var isImage = contentType.StartsWith("image/");
        int? width = null;
        int? height = null;

        if (isImage)
        {
            try
            {
                using var codec = SKCodec.Create(physicalPath);
                if (codec != null)
                {
                    width = codec.Info.Width;
                    height = codec.Info.Height;
                }
                else
                {
                    using var bitmap = SKBitmap.Decode(physicalPath);
                    if (bitmap != null)
                    {
                        width = bitmap.Width;
                        height = bitmap.Height;
                    }
                    else
                    {
                        Logger.LogWarning("Failed to decode image file {PhysicalPath} using SkiaSharp. Treating as non-image.", physicalPath);
                        isImage = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error reading image dimensions for file {PhysicalPath}.", physicalPath);
                isImage = false;
            }
        }

        var asset = new MediaAssetEntity
        {
            OriginalFileName = fileName,
            StoredFileName = storedFileName,
            RelativePath = relativePath,
            ContentType = contentType,
            FileSize = fileStream.Length,
            IsImage = isImage,
            IsVideo = contentType.StartsWith("video/"),
            Width = width,
            Height = height
        };

        Context.MediaAssets.Add(asset);
        await Context.SaveChangesAsync(ct);

        return Map(asset);
    }

    public async Task<MediaAssetDto?> GetAssetAsync(Guid id, CancellationToken ct = default)
    {
        var asset = await Context.MediaAssets
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        return asset != null ? Map(asset) : null;
    }

    public async Task UpdateAssetAsync(UpdateMediaAssetDto dto, CancellationToken ct = default)
    {
        var asset = await Context.MediaAssets.FirstOrDefaultAsync(p => p.Id == dto.Id, ct);
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
        var asset = await Context.MediaAssets.FindAsync([id], cancellationToken: ct);
        if (asset != null)
        {
            asset.IsDeleted = true;
            asset.DeletedAtUtc = DateTime.UtcNow;
            await Context.SaveChangesAsync(ct);
        }
    }

    public async Task RestoreAssetAsync(Guid id, CancellationToken ct = default)
    {
        var asset = await Context.MediaAssets
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
        var asset = await Context.MediaAssets
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (asset != null)
        {
            // Delete file from disk
            var uploadsFolder = Path.Combine(WebRootPath, "uploads");
            var physicalPath = Path.Combine(uploadsFolder, asset.StoredFileName);
            if (File.Exists(physicalPath))
            {
                File.Delete(physicalPath);
            }

            Context.MediaAssets.Remove(asset);
            await Context.SaveChangesAsync(ct);
        }
    }

    public async Task DeleteMediaAsync(IEnumerable<string> relativePaths, CancellationToken ct = default)
    {
        foreach (var path in relativePaths.Distinct())
        {
            var asset = await Context.MediaAssets
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
