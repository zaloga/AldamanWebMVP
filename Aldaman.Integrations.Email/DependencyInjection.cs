using Aldaman.Integrations.Email.Options;
using Aldaman.Integrations.Email.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aldaman.Integrations.Email;

public static class DependencyInjection
{
    public static IServiceCollection AddEmailIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
        services.AddScoped<IEmailService, SmtpEmailService>();

        return services;
    }
}
