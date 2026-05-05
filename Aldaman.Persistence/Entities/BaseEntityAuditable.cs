namespace Aldaman.Persistence.Entities;

/// <summary>
/// Base entity class with common properties for all entities that require auditing and soft deletion.
/// </summary>
public abstract class BaseEntityAuditable
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? CreatedByUserId { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
    public Guid? UpdatedByUserId { get; set; }


    // Navigation properties
    public virtual AppUser? CreatedByUser { get; set; }
    public virtual AppUser? UpdatedByUser { get; set; }
}
