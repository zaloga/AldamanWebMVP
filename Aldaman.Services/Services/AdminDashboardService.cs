using Aldaman.Persistence.Context;
using Aldaman.Services.Dtos.AdminDashboard;
using Aldaman.Services.Dtos.Blog;
using Aldaman.Services.Dtos.ContactMessage;
using Aldaman.Services.Dtos.Media;
using Aldaman.Services.Dtos.Page;
using Aldaman.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aldaman.Services.Services;

internal sealed class AdminDashboardService : IAdminDashboardService
{
    private readonly AppDbContext _context;

    public AdminDashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AdminDashboardStatsDto> GetStatsAsync()
    {
        var totalPages = await _context.ContentPages.CountAsync();
        var totalPosts = await _context.BlogPosts.CountAsync();
        var totalMessages = await _context.ContactMessages.CountAsync();
        var totalMedia = await _context.MediaAssets.CountAsync();
        var totalMediaSize = await _context.MediaAssets.SumAsync(x => x.FileSize);

        var latestMessagesEntities = await _context.ContactMessages
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(5)
            .ToListAsync();

        var latestMessages = latestMessagesEntities.Select(x => new ContactMessageDto
        {
            Id = x.Id,
            EmailOrPhone = x.EmailOrPhone,
            Subject = x.Subject,
            Message = x.Message,
            CreatedAtUtc = x.CreatedAtUtc,
            SentAtUtc = x.SentAtUtc,
            State = x.State
        }).ToList();

        var latestPostsEntities = await _context.BlogPosts
            .Include(x => x.Translations)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(5)
            .ToListAsync();

        var latestPosts = latestPostsEntities.Select(x => new BlogPostListItemDto
        {
            Id = x.Id,
            Title = x.Translations.FirstOrDefault()?.Title ?? "Untitled",
            CreatedAtUtc = x.CreatedAtUtc,
            PublishedAtUtc = x.PublishedAtUtc,
            IsPublished = x.IsPublished
        }).ToList();

        var latestPagesEntities = await _context.ContentPages
            .Include(x => x.Translations)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(5)
            .ToListAsync();

        var latestPages = latestPagesEntities.Select(x => new ContentPageListItemDto
        {
            Id = x.Id,
            Title = x.Translations.FirstOrDefault()?.Title ?? "Untitled",
            PageKey = x.PageKey,
            CreatedAtUtc = x.CreatedAtUtc
        }).ToList();

        var latestMediaEntities = await _context.MediaAssets
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(5)
            .ToListAsync();

        var latestMedia = latestMediaEntities.Select(x => new MediaAssetDto
        {
            Id = x.Id,
            OriginalFileName = x.OriginalFileName,
            RelativePath = x.RelativePath,
            UploadedAtUtc = x.CreatedAtUtc,
            FileSize = x.FileSize,
            ContentType = x.ContentType
        }).ToList();

        return new AdminDashboardStatsDto
        {
            TotalPagesCount = totalPages,
            BlogPostsCount = totalPosts,
            ContactMessagesCount = totalMessages,
            TotalMediaCount = totalMedia,
            TotalMediaSizeInBytes = totalMediaSize,
            RecentMessagesCount = latestMessages.Count,
            LatestMessages = latestMessages,
            LatestBlogPosts = latestPosts,
            LatestPages = latestPages,
            LatestMedia = latestMedia
        };
    }
}
