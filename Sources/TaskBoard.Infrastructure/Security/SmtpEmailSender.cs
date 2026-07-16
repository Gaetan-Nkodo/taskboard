using System.Net.Mail;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TaskBoard.Infrastructure.Security;

public class SmtpEmailSender : IEmailSender
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<EmailSettings> settings, ILogger<SmtpEmailSender> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        _logger.LogInformation("📨 Email envoyé à {Email} via SMTP local (Mailpit)", to);

        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.UseSsl
        };

        var message = new MailMessage(_settings.From, to, subject, body)
        {
            IsBodyHtml = true
        };

        try
        {
            await client.SendMailAsync(message, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur SMTP lors de l'envoi de l'email");
            throw;
        }

        File.WriteAllText("sandbox-email-last.html", body);
    }
}
