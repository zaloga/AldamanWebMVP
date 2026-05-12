using Aldaman.Persistence.Context;
using Aldaman.Persistence.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aldaman.Persistence.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            // Seeding logic (migrations moved to DatabaseMigrator)

            // Seed Roles
            string[] roles = ["SuperAdmin", "Admin", "Editor"];

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new AppRole { Name = roleName });
                }
            }

            // Seed SuperAdmin User
            var superAdminEmail = "superadmin@aldaman.local";
            var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);

            if (superAdminUser == null)
            {
                superAdminUser = new AppUser
                {
                    UserName = superAdminEmail,
                    Email = superAdminEmail,
                    DisplayName = "Super Admin",
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(superAdminUser, "SuperUser123456#");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
                    logger.LogInformation("SuperAdmin user seeded successfully.");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        logger.LogError("Error seeding SuperAdmin: {Error}", error.Description);
                    }
                }
            }

            // Seed Default CSS Style Settings
            var defaultStyles = new List<StyleSettingEntity>
            {
                new() { Key = "--brand-primary", Value = "#a83f23", DefaultValue = "#a83f23", Type = Enums.CssType.Color },
                new() { Key = "--brand-secondary", Value = "#4a332a", DefaultValue = "#4a332a", Type = Enums.CssType.Color },
                new() { Key = "--brand-accent", Value = "#ffb100", DefaultValue = "#ffb100", Type = Enums.CssType.Color },
                new() { Key = "--brand-bg-light", Value = "#fdfaf6", DefaultValue = "#fdfaf6", Type = Enums.CssType.Color },
                new() { Key = "--brand-bg-dark", Value = "#1b1816", DefaultValue = "#1b1816", Type = Enums.CssType.Color }
            };

            foreach (var style in defaultStyles)
            {
                if (!await context.StyleSettings.AnyAsync(s => s.Key == style.Key))
                {
                    context.StyleSettings.Add(style);
                }
            }

            if (context.ChangeTracker.HasChanges())
            {
                await context.SaveChangesAsync();
                logger.LogInformation("Default CSS style settings seeded successfully.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database seeding.");
        }
    }
}
