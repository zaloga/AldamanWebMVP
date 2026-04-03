using Aldaman.Persistence.Entities;
using Aldaman.Services.Dtos.Account;
using Aldaman.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Aldaman.Services.Services;

/// <summary>
/// Implementation of IAccountService using ASP.NET Core Identity.
/// </summary>
public sealed class AccountService : IAccountService
{
    private UserManager<AppUser> UserManager { get; }
    private SignInManager<AppUser> SignInManager { get; }

    public AccountService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        UserManager = userManager;
        SignInManager = signInManager;
    }

    public async Task<LoginResultDto> LoginAsync(string email, string password, bool rememberMe)
    {
        var user = await UserManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new LoginResultDto(false, false, false, false);
        }

        var result = await SignInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);

        return new LoginResultDto(
            result.Succeeded,
            result.IsLockedOut,
            result.IsNotAllowed,
            result.RequiresTwoFactor);
    }

    public async Task LogoutAsync()
    {
        await SignInManager.SignOutAsync();
    }
}
