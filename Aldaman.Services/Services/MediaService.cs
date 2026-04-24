using System.Drawing;
using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Dtos.Media;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aldaman.Services.Services;

public sealed class MediaService : IMediaService
{
    private AppDbContext Context { get; }
    private string WebRootPath { get; }

    public MediaService(AppDbContext context, string webRootPath)
    {
        Context = context;
        WebRootPath = webRootPath;
    }

    public async Task<PagedResultDto<MediaAssetDto>> ListAssetsAsync(PaginationQuery query)
    {
        var dbQuery = Context.MediaAssets
            .AsQueryable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            dbQuery = dbQuery.Where(p => p.OriginalFileName.Contains(query.SearchTerm) || (p.AltText != null && p.AltText.Contains(query.SearchTerm)) || (p.Title != null && p.Title.Contains(query.SearchTerm)));
        }

        // Sorting
        dbQuery = query.SortBy switch
        {
            "FileName" => query.SortDescending ? dbQuery.OrderByDescending(p => p.OriginalFileName) : dbQuery.OrderBy(p => p.OriginalFileName),
            "UploadedAt" => query.SortDescending ? dbQuery.OrderByDescending(p => p.CreatedAtUtc) : dbQuery.OrderBy(p => p.CreatedAtUtc),
            "Size" => query.SortDescending ? dbQuery.OrderByDescending(p => p.FileSize) : dbQuery.OrderBy(p => p.FileSize),
            _ => dbQuery.OrderByDescending(p => p.CreatedAtUtc)
        };

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => Map(p))
            .ToListAsync();

        return new PagedResultDto<MediaAssetDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<MediaAssetDto> UploadAsync(Stream fileStream, string fileName, string contentType)
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
            await fileStream.CopyToAsync(fs);
        }

        var isImage = contentType.StartsWith("image/");
        int? width = null;
        int? height = null;

        if (isImage)
        {
            try
            {
                using (var image = Image.FromFile(physicalPath))
                {
                    width = image.Width;
                    height = image.Height;
                }
            }
            catch
            {
                // Silently fail if not a valid image
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
        await Context.SaveChangesAsync();

        return Map(asset);
    }

    public async Task<MediaAssetDto?> GetAssetAsync(Guid id)
    {
        var asset = await Context.MediaAssets.FirstOrDefaultAsync(p => p.Id == id);
        return asset != null ? Map(asset) : null;
    }

    public async Task UpdateAssetAsync(UpdateMediaAssetDto dto)
    {
        var asset = await Context.MediaAssets.FirstOrDefaultAsync(p => p.Id == dto.Id);
        if (asset != null)
        {
            asset.AltText = dto.AltText;
            asset.Title = dto.Title;
            asset.UpdatedAtUtc = DateTime.UtcNow;

            await Context.SaveChangesAsync();
        }
    }

    public async Task DeleteAssetAsync(Guid id)
    {
        var asset = await Context.MediaAssets.FindAsync(id);
        if (asset != null)
        {
            asset.IsDeleted = true;
            asset.DeletedAtUtc = DateTime.UtcNow;
            await Context.SaveChangesAsync();
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
            AltText = p.AltText,
            Title = p.Title,
            UploadedAtUtc = p.CreatedAtUtc,
            IsImage = p.IsImage,
            IsVideo = p.IsVideo
        };
    }
}
