namespace Aldaman.Persistence.Entities;

/// <summary>
/// Root base entity class with primary key.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}
