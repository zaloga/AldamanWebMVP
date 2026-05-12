using Aldaman.Persistence.Context;
using Aldaman.Services.Dtos.AdminDashboard;
using Aldaman.Services.Dtos.ContactMessage;
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

        return new AdminDashboardStatsDto
        {
            TotalPagesCount = totalPages,
            BlogPostsCount = totalPosts,
            ContactMessagesCount = totalMessages,
            TotalMediaCount = totalMedia,
            TotalMediaSizeInBytes = totalMediaSize,
            RecentMessagesCount = latestMessages.Count,
            LatestMessages = latestMessages
        };
    }
}
