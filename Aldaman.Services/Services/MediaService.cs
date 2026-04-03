using Aldaman.Persistence.Context;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Dtos.Media;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aldaman.Services.Services;

public sealed class MediaService : IMediaService
{
    private AppDbContext Context { get; }

    public MediaService(AppDbContext context)
    {
        Context = context;
    }

    public async Task<PagedResultDto<MediaAssetDto>> ListAssetsAsync(PaginationQuery query)
    {
        var dbQuery = Context.MediaAssets.AsQueryable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            dbQuery = dbQuery.Where(p => p.OriginalFileName.Contains(query.SearchTerm) || (p.AltText != null && p.AltText.Contains(query.SearchTerm)) || (p.Title != null && p.Title.Contains(query.SearchTerm)));
        }

        // Sorting
        dbQuery = query.SortBy switch
        {
            "FileName" => query.SortDescending ? dbQuery.OrderByDescending(p => p.OriginalFileName) : dbQuery.OrderBy(p => p.OriginalFileName),
            "UploadedAt" => query.SortDescending ? dbQuery.OrderByDescending(p => p.UploadedAtUtc) : dbQuery.OrderBy(p => p.UploadedAtUtc),
            "Size" => query.SortDescending ? dbQuery.OrderByDescending(p => p.FileSize) : dbQuery.OrderBy(p => p.FileSize),
            _ => dbQuery.OrderByDescending(p => p.UploadedAtUtc)
        };

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new MediaAssetDto
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
                UploadedAtUtc = p.UploadedAtUtc,
                IsImage = p.IsImage,
                IsVideo = p.IsVideo,
                IsActive = p.IsActive
            })
            .ToListAsync();

        return new PagedResultDto<MediaAssetDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public Task<MediaAssetDto> UploadAsync(Stream fileStream, string fileName, string contentType) => throw new NotImplementedException();
    public Task<MediaAssetDto?> GetAssetAsync(Guid id) => throw new NotImplementedException();

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
}
