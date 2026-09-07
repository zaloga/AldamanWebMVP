namespace Aldaman.Persistence.Entities;

/// <summary>
/// Base entity class with common properties for all entities that require creation and update auditing.
/// </summary>
public abstract class BaseEntityAuditable : BaseEntityCreatable
{
    public DateTime? UpdatedAtUtc { get; set; }
    public Guid? UpdatedByUserId { get; set; }

    // Navigation properties
    public virtual AppUser? UpdatedByUser { get; set; }
}
