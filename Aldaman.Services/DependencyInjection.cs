using Aldaman.Services.Configuration;
using Aldaman.Services.Interfaces;
using Aldaman.Services.Services;
using Aldaman.Services.Services.Images;
using Microsoft.Extensions.DependencyInjection;

namespace Aldaman.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, string webRootPath)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IBlogService, BlogService>();
        services.AddScoped<IContentPageService, ContentPageService>();
        services.AddScoped<IMediaService>(sp => new MediaService(
            sp.GetRequiredService<Aldaman.Persistence.Context.AppDbContext>(),
            webRootPath,
            sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<MediaService>>()));
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IAdminDashboardService, AdminDashboardService>();
        services.AddScoped<IStyleService, StyleService>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddSingleton<IHtmlSanitizerService, HtmlSanitizerService>();
        services.AddOptions<ImageProcessingSettings>().BindConfiguration(ImageProcessingSettings.SectionName);
        services.AddScoped<IImageProcessingService, SkiaImageProcessingService>();

        // Register other services here as they are implemented

        return services;
    }
}
