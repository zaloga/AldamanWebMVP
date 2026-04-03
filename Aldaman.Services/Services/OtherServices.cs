using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Aldaman.Services.Dtos.AdminDashboard;
using Aldaman.Services.Dtos.ContactMessage;
using Aldaman.Services.Dtos.General;
using Aldaman.Services.Dtos.Media;
using Aldaman.Services.Dtos.SiteConfiguration;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aldaman.Services.Services;

public sealed class MediaService : IMediaService
{
    private readonly AppDbContext _context;

    public MediaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<MediaAssetDto>> ListAssetsAsync(PaginationQuery query)
    {
        var dbQuery = _context.MediaAssets.AsQueryable();

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
        var asset = await _context.MediaAssets.FindAsync(id);
        if (asset != null)
        {
            asset.IsDeleted = true;
            asset.DeletedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}

public sealed class ContactService : IContactService
{
    private readonly AppDbContext _context;

    public ContactService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResultDto<ContactMessageDto>> GetPagedMessagesAsync(PaginationQuery query)
    {
        var dbQuery = _context.ContactMessages.AsQueryable();

        // Filtering
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            dbQuery = dbQuery.Where(p => p.Name.Contains(query.SearchTerm) || p.Email.Contains(query.SearchTerm) || (p.Subject != null && p.Subject.Contains(query.SearchTerm)));
        }

        // Sorting
        dbQuery = query.SortBy switch
        {
            "Name" => query.SortDescending ? dbQuery.OrderByDescending(p => p.Name) : dbQuery.OrderBy(p => p.Name),
            "CreatedAt" => query.SortDescending ? dbQuery.OrderByDescending(p => p.CreatedAtUtc) : dbQuery.OrderBy(p => p.CreatedAtUtc),
            "State" => query.SortDescending ? dbQuery.OrderByDescending(p => p.State) : dbQuery.OrderBy(p => p.State),
            _ => dbQuery.OrderByDescending(p => p.CreatedAtUtc)
        };

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ContactMessageDto
            {
                Id = p.Id,
                Name = p.Name,
                Email = p.Email,
                Phone = p.Phone,
                Subject = p.Subject,
                Message = p.Message,
                CreatedAtUtc = p.CreatedAtUtc,
                SentAtUtc = p.SentAtUtc,
                State = (ContactMessageState)p.State
            })
            .ToListAsync();

        return new PagedResultDto<ContactMessageDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public Task SubmitMessageAsync(ContactMessageDto dto, string clientIp, string userAgent) => throw new NotImplementedException();
    public Task MarkAsHandledAsync(Guid id) => throw new NotImplementedException();

    public async Task DeleteMessageAsync(Guid id)
    {
        var message = await _context.ContactMessages.FindAsync(id);
        if (message != null)
        {
            message.IsDeleted = true;
            message.DeletedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public Task<IEnumerable<ContactMessageDto>> GetRecentMessagesAsync() => throw new NotImplementedException();
}

public sealed class SiteConfigurationService : ISiteConfigurationService
{
    public Task<SiteConfigurationDto> GetConfigurationAsync() => throw new NotImplementedException();
    public Task UpdateConfigurationAsync(SiteConfigurationDto dto) => throw new NotImplementedException();
}

public sealed class AdminDashboardService : IAdminDashboardService
{
    public Task<AdminDashboardStatsDto> GetStatsAsync() => throw new NotImplementedException();
}
