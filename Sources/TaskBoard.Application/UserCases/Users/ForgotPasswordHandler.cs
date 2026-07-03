using MediatR;

using Microsoft.Extensions.Configuration;

using TaskBoard.Application.Common.Emails;
using TaskBoard.Application.Requests;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordRequest>
{
    private readonly IUserRepository _users;
    private readonly IPasswordResetTokenRepository _tokens;
    private readonly IEmailSender _emailSender;
    private readonly string _frontendUrl;

    public ForgotPasswordHandler(
        IUserRepository users,
        IPasswordResetTokenRepository tokens,
        IEmailSender emailSender,
        IConfiguration config)
    {
        _users = users;
        _tokens = tokens;
        _emailSender = emailSender;

        _frontendUrl = config["Frontend:BaseUrl"] ?? "https://taskboard.app";
    }

    public async Task Handle(ForgotPasswordRequest request, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(request.Email, ct);

        if (user is null)
            return;

        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("=", "").Replace("+", "").Replace("/", "");

        var resetToken = new PasswordResetToken(
            user.Id,
            token,
            DateTime.UtcNow.AddMinutes(15)
        );

        await _tokens.AddAsync(resetToken, ct);
        await _tokens.SaveChangesAsync(ct);

        var link = $"{_frontendUrl}/reset-password?token={token}";
        var body = EmailTemplates.ResetPassword(link);

        await _emailSender.SendAsync(
            user.Email,
            "Réinitialisation du mot de passe",
            body
        );
    }
}
