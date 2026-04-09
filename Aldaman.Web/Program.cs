using System.Globalization;
using Aldaman.Persistence;
using Aldaman.Persistence.Migrator;
using Aldaman.Persistence.Seed;
using Aldaman.Services;
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

        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

        CultureInfo[] supportedCultures =
        [
            new("cs"), // TODO : Move culture codes to configuration
            new("en")
        ];

        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("cs"); // TODO : Move default culture code to configuration
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

        app.MapControllerRoute( // TODO ^cs|en$ generate regex from supported cultures configuration
            name: "areas_localized",
            pattern: "{culture:regex(^cs|en$)}/{area:exists}/{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "page_detail_localized",
            pattern: "{culture:regex(^cs|en$)}/page/{slug}",
            defaults: new { controller = "Page", action = "Detail" })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "blog_index_localized",
            pattern: "{culture:regex(^cs|en$)}/blog",
            defaults: new { controller = "Blog", action = "Index" })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "blog_detail_localized",
            pattern: "{culture:regex(^cs|en$)}/blog/{slug}",
            defaults: new { controller = "Blog", action = "Detail" })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "contact_index_localized",
            pattern: "{culture:regex(^cs|en$)}/contact",
            defaults: new { controller = "Contact", action = "Index" })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "contact_submit_localized",
            pattern: "{culture:regex(^cs|en$)}/contact/send",
            defaults: new { controller = "Contact", action = "Submit" })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "contact_success_localized",
            pattern: "{culture:regex(^cs|en$)}/contact/success",
            defaults: new { controller = "Contact", action = "Success" })
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "default_localized",
            pattern: "{culture:regex(^cs|en$)}/{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}