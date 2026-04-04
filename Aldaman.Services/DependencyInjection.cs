using Aldaman.Services.Interfaces;
using Aldaman.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aldaman.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, string webRootPath)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IBlogService, BlogService>();
        services.AddScoped<IPageService, PageService>();
        services.AddScoped<IMediaService>(sp => new MediaService(sp.GetRequiredService<Aldaman.Persistence.Context.AppDbContext>(), webRootPath));
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<IAdminDashboardService, AdminDashboardService>();

        // Register other services here as they are implemented

        return services;
    }
}
