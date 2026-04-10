using System.Net;
using System.Net.Mail;
using Aldaman.Integrations.Email.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aldaman.Integrations.Email.Services;

/// <summary>
/// SMTP implementation of IEmailService using System.Net.Mail.
/// </summary>
internal sealed class SmtpEmailService : IEmailService
{
    private EmailOptions Options { get; }
    private ILogger<SmtpEmailService> Logger { get; }

    public SmtpEmailService(IOptions<EmailOptions> options, ILogger<SmtpEmailService> logger)
    {
        Options = options.Value;
        Logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default)
    {
        try
        {
            using SmtpClient client = new(Options.SmtpHost, Options.SmtpPort)
            {
                Credentials = new NetworkCredential(Options.SmtpUsername, Options.SmtpPassword),
                EnableSsl = Options.EnableSsl
            };

            using MailMessage mailMessage = new()
            {
                From = new MailAddress(Options.FromEmail, Options.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(to);

            // SmtpClient.SendMailAsync does not accept CancellationToken in older .NET versions, 
            // but in .NET 10 it should be available or we can use Task.Run with token.
            // For now, we'll use the available async overload.
            await client.SendMailAsync(mailMessage, cancellationToken);

            Logger.LogInformation("Email sent successfully to {Recipient}", to);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to send email to {Recipient}", to);
            throw; // Rethrow to let the caller handle the failure state in DB if needed
        }
    }
}
