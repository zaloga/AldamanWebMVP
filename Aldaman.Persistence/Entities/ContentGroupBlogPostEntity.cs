namespace Aldaman.Persistence.Entities;

/// <summary>
/// M:N join entity between ContentGroup and BlogPost.
/// </summary>
public class ContentGroupBlogPostEntity : BaseEntityAuditable
{
    public Guid ContentGroupId { get; set; }
    public Guid BlogPostId { get; set; }

    /// <summary>
    /// Order of the blog post inside this content group.
    /// </summary>
    public int Order { get; set; }

    // Navigation properties
    public virtual ContentGroupEntity ContentGroup { get; set; } = null!;
    public virtual BlogPostEntity BlogPost { get; set; } = null!;
}
