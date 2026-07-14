using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using TaskBoard.Application.Common;
using TaskBoard.Application.Common.Emails;
using TaskBoard.Application.Services;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

public class ResendConfirmationEmailHandler
{
    private readonly IUserRepository _users;
    private readonly IEmailVerificationTokenRepository _tokens;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<ResendConfirmationEmailHandler> _logger;
    private readonly string _frontendUrl;

    public ResendConfirmationEmailHandler(
        IUserRepository users,
        IEmailVerificationTokenRepository tokens,
        IEmailSender emailSender,
        IConfiguration config,
        ILogger<ResendConfirmationEmailHandler> logger)
    {
        _users = users;
        _tokens = tokens;
        _emailSender = emailSender;
        _logger = logger;

        _frontendUrl = FrontendUrlHelper.GetNormalizedFrontendUrl(config);
    }

    public async Task Handle(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email is required.");

        var user = await _users.GetByEmailAsync(email);
        if (user == null)
            throw new DomainException("User not found.");

        if (user.EmailConfirmed)
            throw new DomainException("Email already confirmed.");

        var existingToken = await _tokens.GetLatestForUserAsync(user.Id);
        string tokenValue;

        if (existingToken != null && !existingToken.IsExpired())
        {
            tokenValue = existingToken.Token;
            _logger.LogInformation(
                "Reusing existing confirmation token for user {UserId}",
                user.Id
            );
        }
        else
        {
            tokenValue = Guid.NewGuid().ToString("N");
            var newToken = new EmailVerificationToken(
                user.Id,
                tokenValue,
                DateTime.UtcNow.AddHours(24)
            );
            await _tokens.AddAsync(newToken);

            _logger.LogInformation(
                "Generated new confirmation token for user {UserId}",
                user.Id
            );
        }

        var confirmUrl = $"{_frontendUrl}/confirm-email?token={tokenValue}";
        var body = EmailTemplates.ConfirmEmail(user.DisplayName, confirmUrl);

        try
        {
            await _emailSender.SendAsync(
                user.Email,
                "Confirme ton email",
                body
            );

            _logger.LogInformation(
                "Resent confirmation email to {Email} with token {Token}",
                user.Email,
                tokenValue
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to resend confirmation email to {Email}",
                user.Email
            );

            throw new DomainException("Failed to resend confirmation email. Please try again later.");
        }
    }
}
