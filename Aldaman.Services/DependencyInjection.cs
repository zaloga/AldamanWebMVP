using Aldaman.Services.Interfaces;
using Aldaman.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aldaman.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();

        // Register other services here as they are implemented

        return services;
    }
}
