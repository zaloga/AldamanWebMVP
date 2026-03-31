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
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<LoginResultDto> LoginAsync(string email, string password, bool rememberMe)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new LoginResultDto(false, false, false, false);
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);

        return new LoginResultDto(
            result.Succeeded,
            result.IsLockedOut,
            result.IsNotAllowed,
            result.RequiresTwoFactor);
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
