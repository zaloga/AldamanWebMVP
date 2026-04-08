using Aldaman.Persistence;
using Aldaman.Persistence.Migrator;
using Aldaman.Persistence.Seed;
using Aldaman.Services;
using Aldaman.Web.Extensions;
using Aldaman.Web.Middleware;
using FluentValidation;
using Serilog;

namespace Aldaman.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Serilog
        builder.Host.UseSerilog((context, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(context.Configuration));

        // Add services to the container.
        builder.Services.AddPersistence(builder.Configuration);

        // Add Identity and Authorization
        builder.Services.AddApplicationIdentity();
        builder.Services.ConfigureApplicationCookie();
        builder.Services.AddApplicationAuthorization();

        builder.Services.AddApplicationServices(builder.Environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));

        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<Aldaman.Persistence.Interfaces.IUserContext, Aldaman.Web.Infrastructure.WebUserContext>();

        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Database migrations and seeding
        await DatabaseMigrator.MigrateAsync(app.Services);
        await DbSeeder.SeedAsync(app.Services);

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseMiddleware<GlobalExceptionMiddleware>();

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}
