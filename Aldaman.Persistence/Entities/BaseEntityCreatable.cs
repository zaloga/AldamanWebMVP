namespace Aldaman.Persistence.Entities;

/// <summary>
/// Base entity class for entities that require creation auditing.
/// </summary>
public abstract class BaseEntityCreatable : BaseEntity
{
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? CreatedByUserId { get; set; }

    // Navigation properties
    public virtual AppUser? CreatedByUser { get; set; }
}
