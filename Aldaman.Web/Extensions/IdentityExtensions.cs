using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Microsoft.AspNetCore.Identity;

namespace Aldaman.Web.Extensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddApplicationIdentity(this IServiceCollection services)
    {
        services.AddIdentity<AppUser, AppRole>(options =>
            {
                // Password policy
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;

                // SignIn settings
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection ConfigureApplicationCookie(this IServiceCollection services)
    {
        services.ConfigureApplicationCookie(options =>
        {
            // Cookie settings
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.Name = "Aldaman.Identity";

            options.ExpireTimeSpan = TimeSpan.FromDays(14);
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.SlidingExpiration = true;
        });

        return services;
    }

    public static IServiceCollection AddApplicationAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("SuperAdminPolicy", policy => policy.RequireRole("SuperAdmin"));
            options.AddPolicy("AdminPolicy", policy => policy.RequireRole("SuperAdmin", "Admin"));
            options.AddPolicy("EditorPolicy", policy => policy.RequireRole("SuperAdmin", "Admin", "Editor"));
        });

        return services;
    }
}
