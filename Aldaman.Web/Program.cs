using System.Globalization;
using Aldaman.Persistence;
using Aldaman.Persistence.Migrator;
using Aldaman.Persistence.Seed;
using Aldaman.Services;
using Aldaman.Integrations.Email;
using Aldaman.Web.Configuration;
using Aldaman.Web.Extensions;
using Aldaman.Web.Middleware;
using FluentValidation;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Serilog;

namespace Aldaman.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Configure Serilog
        builder.Host.UseSerilog((context, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(context.Configuration));

        // Add services to the container.
        builder.Services.AddPersistence(builder.Configuration);
        builder.Services.AddEmailIntegration(builder.Configuration);

        // Add Identity and Authorization
        builder.Services.AddApplicationIdentity();
        builder.Services.ConfigureApplicationCookie();
        builder.Services.AddApplicationAuthorization();

        builder.Services.AddApplicationServices(
            builder.Environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));

        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        builder.Services.AddValidatorsFromAssemblyContaining<Aldaman.Services.Validators.Media.UpdateMediaAssetDtoValidator>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<Aldaman.Persistence.Interfaces.IUserContext, Aldaman.Web.Infrastructure.WebUserContext>();

        builder.Services.AddLocalization();

        LocalizationSettings localizationSettings = builder.Configuration
            .GetSection(LocalizationSettings.SectionName)
            .Get<LocalizationSettings>()!;

        CultureInfo[] supportedCultures = localizationSettings.SupportedCultures
            .Select(c => new CultureInfo(c))
            .ToArray();

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(localizationSettings.DefaultCulture);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.ApplyCurrentCultureToResponseHeaders = true;

            options.RequestCultureProviders.Clear();
            options.RequestCultureProviders.Add(new RouteDataRequestCultureProvider
            {
                RouteDataStringKey = "culture",
                UIRouteDataStringKey = "culture"
            });
        });

        builder.Services.AddControllersWithViews()
            .AddViewLocalization()
            .AddDataAnnotationsLocalization();

        WebApplication app = builder.Build();

        // Database migrations and seeding
        await DatabaseMigrator.MigrateAsync(app.Services);
        await DbSeeder.SeedAsync(app.Services);

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseMiddleware<GlobalExceptionMiddleware>();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseRequestLocalization();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();

        string cultureRegex = $"^({string.Join("|", localizationSettings.SupportedCultures)})$";

        app.MapControllerRoute(
            name: "areas_localized",
            pattern: "{culture:regex(" + cultureRegex + ")}/{area:exists}/{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}",
            defaults: new { culture = localizationSettings.DefaultCulture })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "page_detail_localized",
            pattern: "{culture:regex(" + cultureRegex + ")}/page/{slug}",
            defaults: new { controller = "Page", action = "Detail" })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "page_detail",
            pattern: "page/{slug}",
            defaults: new { controller = "Page", action = "Detail", culture = localizationSettings.DefaultCulture })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "blog_index_localized",
            pattern: "{culture:regex(" + cultureRegex + ")}/blog",
            defaults: new { controller = "Blog", action = "Index" })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "blog_index",
            pattern: "blog",
            defaults: new { controller = "Blog", action = "Index", culture = localizationSettings.DefaultCulture })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "blog_detail_localized",
            pattern: "{culture:regex(" + cultureRegex + ")}/blog/{slug}",
            defaults: new { controller = "Blog", action = "Detail" })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "blog_detail",
            pattern: "blog/{slug}",
            defaults: new { controller = "Blog", action = "Detail", culture = localizationSettings.DefaultCulture })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "contact_index_localized",
            pattern: "{culture:regex(" + cultureRegex + ")}/contact",
            defaults: new { controller = "Contact", action = "Index" })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "contact_index",
            pattern: "contact",
            defaults: new { controller = "Contact", action = "Index", culture = localizationSettings.DefaultCulture })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "contact_submit_localized",
            pattern: "{culture:regex(" + cultureRegex + ")}/contact/send",
            defaults: new { controller = "Contact", action = "Submit" })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "contact_submit",
            pattern: "contact/send",
            defaults: new { controller = "Contact", action = "Submit", culture = localizationSettings.DefaultCulture })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "contact_success_localized",
            pattern: "{culture:regex(" + cultureRegex + ")}/contact/success",
            defaults: new { controller = "Contact", action = "Success" })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "contact_success",
            pattern: "contact/success",
            defaults: new { controller = "Contact", action = "Success", culture = localizationSettings.DefaultCulture })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "default_localized",
            pattern: "{culture:regex(" + cultureRegex + ")}/{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}",
            defaults: new { culture = localizationSettings.DefaultCulture })
            .WithStaticAssets();

        app.Run();
    }
}