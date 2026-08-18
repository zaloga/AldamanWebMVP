namespace Aldaman.Persistence.Entities;

/// <summary>
/// M:N join entity between ContentGroup and ContentPage.
/// </summary>
public class ContentGroupContentPageEntity : BaseEntityAuditable
{
    public Guid ContentGroupId { get; set; }
    public Guid ContentPageId { get; set; }

    /// <summary>
    /// Order of the page inside this content group.
    /// </summary>
    public int Order { get; set; }

    // Navigation properties
    public virtual ContentGroupEntity ContentGroup { get; set; } = null!;
    public virtual ContentPageEntity ContentPage { get; set; } = null!;
}
