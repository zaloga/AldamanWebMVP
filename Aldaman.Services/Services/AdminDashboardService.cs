using Aldaman.Persistence.Context;
using Aldaman.Services.Dtos.AdminDashboard;
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

    public async Task<AdminDashboardStatsDto> GetStatsAsync(CancellationToken ct = default)
    {
        var totalPages = await _context.ContentPages.CountAsync(ct);
        var totalPosts = await _context.BlogPosts.CountAsync(ct);
        var totalMessages = await _context.ContactMessages.CountAsync(ct);
        var totalMedia = await _context.MediaAssets.CountAsync(ct);
        var totalMediaSize = await _context.MediaAssets.SumAsync(x => x.FileSize, ct);

        return new AdminDashboardStatsDto
        {
            TotalPagesCount = totalPages,
            BlogPostsCount = totalPosts,
            ContactMessagesCount = totalMessages,
            TotalMediaCount = totalMedia,
            TotalMediaSizeInBytes = totalMediaSize
        };
    }
}
