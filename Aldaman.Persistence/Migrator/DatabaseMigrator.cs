using Aldaman.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aldaman.Persistence.Migrator;

public static class DatabaseMigrator
{
    public static async Task MigrateAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                logger.LogInformation("Applying pending migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Migrations applied successfully.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during database migration.");
            throw;
        }
    }
}
