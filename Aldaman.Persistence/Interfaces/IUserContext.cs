namespace Aldaman.Persistence.Interfaces;

public interface IUserContext
{
    Guid? CurrentUserId { get; }
}
