namespace Aldaman.Persistence.Entities;

/// <summary>
/// Base entity class for entities that require creation auditing and soft deletion, but not update auditing.
/// </summary>
public abstract class BaseEntityCreatableSoftDel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? CreatedByUserId { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? DeletedByUserId { get; set; }

    // Navigation properties
    public virtual AppUser? CreatedByUser { get; set; }
    public virtual AppUser? DeletedByUser { get; set; }
}
