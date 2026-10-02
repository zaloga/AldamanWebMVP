using Aldaman.Services.Configuration;
using Aldaman.Services.Interfaces;
using Aldaman.Services.Services;
using Aldaman.Services.Services.Images;
using Aldaman.Persistence.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Aldaman.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IContentService, ContentService>();
        services.AddScoped<IContentGroupService, ContentGroupService>();
        services.AddScoped<INavigationService, NavigationService>();

        // Media Storage configuration and keyed services
        services.AddOptions<MediaStorageSettings>().BindConfiguration(MediaStorageSettings.SectionName);

        services.AddKeyedScoped<IFileStorageService, FileSystemStorageService>(StorageProviderType.FileSystem);

        services.AddKeyedScoped<IFileStorageService, CloudflareR2StorageService>(
            StorageProviderType.CloudflareR2);

        // Default storage service based on MediaStorageSettings.Provider
        services.AddScoped<IFileStorageService>(sp =>
        {
            MediaStorageSettings settings = sp.GetRequiredService<IOptions<MediaStorageSettings>>().Value;
            return sp.GetKeyedService<IFileStorageService>(settings.Provider)
                ?? sp.GetRequiredKeyedService<IFileStorageService>(StorageProviderType.FileSystem);
        });

        services.AddScoped<IMediaService, MediaService>();

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
