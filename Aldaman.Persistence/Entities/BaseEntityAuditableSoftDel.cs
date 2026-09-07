namespace Aldaman.Persistence.Entities;

/// <summary>
/// Base entity class with common properties for all entities that require auditing and soft deletion.
/// </summary>
public abstract class BaseEntityAuditableSoftDel : BaseEntityAuditable
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? DeletedByUserId { get; set; }

    // Navigation properties
    public virtual AppUser? DeletedByUser { get; set; }
}
