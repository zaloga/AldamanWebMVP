namespace Aldaman.Services.Dtos.AdminDashboard;

/// <summary>
/// Statistics for the admin dashboard.
/// </summary>
public class AdminDashboardStatsDto
{
    public int TotalPagesCount { get; set; }
    public int TotalContentGroupsCount { get; set; }
    public int BlogPostsCount { get; set; }
    public int ContactMessagesCount { get; set; }
    public long TotalMediaCount { get; set; }
    public long TotalMediaSizeInBytes { get; set; }
}
