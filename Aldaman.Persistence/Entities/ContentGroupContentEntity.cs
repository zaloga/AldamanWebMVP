namespace Aldaman.Persistence.Entities;

/// <summary>
/// M:N join entity between ContentGroup and ContentEntity.
/// </summary>
public class ContentGroupContentEntity : BaseEntityAuditable
{
    public Guid ContentGroupId { get; set; }
    public Guid ContentId { get; set; }

    /// <summary>
    /// Order of the content item inside this content group.
    /// </summary>
    public int Order { get; set; }

    // Navigation properties
    public virtual ContentGroupEntity ContentGroup { get; set; } = null!;
    public virtual ContentEntity Content { get; set; } = null!;
}
