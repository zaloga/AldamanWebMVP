using Aldaman.Services.Dtos.Blog;
using Aldaman.Services.Dtos.ContactMessage;
using Aldaman.Services.Dtos.Media;
using Aldaman.Services.Dtos.Page;

namespace Aldaman.Services.Dtos.AdminDashboard;

/// <summary>
/// Statistics for the admin dashboard.
/// </summary>
public class AdminDashboardStatsDto
{
    public int TotalPagesCount { get; set; }
    public int BlogPostsCount { get; set; }
    public int ContactMessagesCount { get; set; }
    public long TotalMediaCount { get; set; }
    public long TotalMediaSizeInBytes { get; set; }
    public int RecentMessagesCount { get; set; }
    public List<ContactMessageDto> LatestMessages { get; set; } = new();
    public List<BlogPostListItemDto> LatestBlogPosts { get; set; } = new();
    public List<ContentPageListItemDto> LatestPages { get; set; } = new();
    public List<MediaAssetDto> LatestMedia { get; set; } = new();
}
