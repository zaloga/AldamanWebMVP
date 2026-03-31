using Aldaman.Services.Dtos.Account;

namespace Aldaman.Services.Interfaces;

/// <summary>
/// Service for managing user authentication and account operations.
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Attempts to sign in a user with email and password.
    /// </summary>
    Task<LoginResultDto> LoginAsync(string email, string password, bool rememberMe);

    /// <summary>
    /// Signs out the current user.
    /// </summary>
    Task LogoutAsync();
}
