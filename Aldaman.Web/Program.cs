using System.Globalization;
using Aldaman.Integrations.Email;
using Aldaman.Persistence;
using Aldaman.Persistence.Migrator;
using Aldaman.Persistence.Seed;
using Aldaman.Services;
using Aldaman.Services.Configuration;
using Aldaman.Web.Extensions;
using Aldaman.Web.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
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
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddFluentValidationClientsideAdapters();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<Aldaman.Persistence.Interfaces.IUserContext, Aldaman.Web.Infrastructure.WebUserContext>();

        // Localization configuration
        builder.Services.AddLocalization();

        var localizationSection = builder.Configuration.GetSection(LocalizationSettings.SectionName);
        builder.Services.Configure<LocalizationSettings>(localizationSection);

        var localizationSettings = localizationSection.Get<LocalizationSettings>()!;
        var supportedCultures = localizationSettings.SupportedCultures
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

        builder.Services.AddAntiforgery(options =>
        {
            options.HeaderName = "RequestVerificationToken";
        });

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

        // Ensure culture is in URL - redirect if missing
        // Moved after UseRouting so it can be route-aware
        app.UseMiddleware<LocalizationRedirectMiddleware>();

        app.UseRequestLocalization();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();

        string cultureRegex = $"^({string.Join("|", localizationSettings.SupportedCultures)})$";
        string culturePattern = "{culture:regex(" + cultureRegex + ")}";

        // Admin and Areas
        app.MapControllerRoute(
            name: "admin_media_upload_quill",
            pattern: "Admin/Media/UploadQuill",
            defaults: new { area = "Admin", controller = "Media", action = "UploadQuill" })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "areas_localized",
            pattern: $"{culturePattern}/{{area:exists}}/{{controller=Home}}/{{action=Index}}/{{id?}}")
            .WithStaticAssets();

        // Content Pages
        app.MapControllerRoute(
            name: "page_detail",
            pattern: $"{culturePattern}/page/{{slug}}",
            defaults: new { controller = "ContentPage", action = "Detail" })
            .WithStaticAssets();

        // Blog
        app.MapControllerRoute(
            name: "blog_index",
            pattern: $"{culturePattern}/blog",
            defaults: new { controller = "Blog", action = "Index" })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "blog_detail",
            pattern: $"{culturePattern}/blog/{{slug}}",
            defaults: new { controller = "Blog", action = "Detail" })
            .WithStaticAssets();

        // Contact
        app.MapControllerRoute(
            name: "contact_index",
            pattern: $"{culturePattern}/contact",
            defaults: new { controller = "Contact", action = "Index" })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "contact_submit",
            pattern: $"{culturePattern}/contact/send",
            defaults: new { controller = "Contact", action = "Submit" })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "contact_success",
            pattern: $"{culturePattern}/contact/success",
            defaults: new { controller = "Contact", action = "Success" })
            .WithStaticAssets();

        // Default Route
        app.MapControllerRoute(
            name: "default",
            pattern: $"{culturePattern}/{{controller=Home}}/{{action=Index}}/{{id?}}")
            .WithStaticAssets();

        app.Run();
    }
}