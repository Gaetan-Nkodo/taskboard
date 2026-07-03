using Microsoft.Extensions.Configuration;

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
    private readonly string _frontendUrl;

    public ResendConfirmationEmailHandler(
        IUserRepository users,
        IEmailVerificationTokenRepository tokens,
        IEmailSender emailSender,
        IConfiguration config)
    {
        _users = users;
        _tokens = tokens;
        _emailSender = emailSender;
        _frontendUrl = config["Frontend:BaseUrl"] ?? "https://taskboard.app";
    }

    public async Task Handle(string email)
    {
        var user = await _users.GetByEmailAsync(email);
        if (user == null)
            throw new DomainException("User not found.");

        if (user.EmailConfirmed)
            throw new DomainException("Email already confirmed.");

        // Vérifier si un token existe déjà
        var existingToken = await _tokens.GetLatestForUserAsync(user.Id);
        string tokenValue;

        if (existingToken != null && !existingToken.IsExpired())
        {
            tokenValue = existingToken.Token;
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
        }

        var confirmUrl = $"{_frontendUrl}/confirm-email?token={tokenValue}";
        var body = EmailTemplates.ConfirmEmail(user.DisplayName, confirmUrl);

        await _emailSender.SendAsync(
            user.Email,
            "Confirme ton email",
            body
        );
    }
}
