namespace Aldaman.Persistence.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? CreatedByUserId { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
    public Guid? UpdatedByUserId { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? DeletedByUserId { get; set; }


    // Navigation properties
    public virtual AppUser? CreatedByUser { get; set; }
    public virtual AppUser? UpdatedByUser { get; set; }
    public virtual AppUser? DeletedByUser { get; set; }
}
