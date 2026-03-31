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
            // Run migrations
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }

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

                var result = await userManager.CreateAsync(superAdminUser, "superuser123456");

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
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database seeding.");
        }
    }
}
