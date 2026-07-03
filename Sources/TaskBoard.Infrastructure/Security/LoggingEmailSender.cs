using Microsoft.Extensions.Logging;

namespace TaskBoard.Infrastructure.Security;

public class LoggingEmailSender : IEmailSender
{
    private readonly ILogger<LoggingEmailSender> _logger;

    public LoggingEmailSender(ILogger<LoggingEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        _logger.LogInformation("Sending email to {To}: {Subject}\n{Body}", to, subject, body);
        return Task.CompletedTask;
    }
}
