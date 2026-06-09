using MediatR;

using TaskBoard.Application.Requests;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordRequest>
{
    private readonly IUserRepository _users;
    private readonly IPasswordResetTokenRepository _tokens;
    private readonly IEmailService _email;

    public ForgotPasswordHandler(
        IUserRepository users,
        IPasswordResetTokenRepository tokens,
        IEmailService email)
    {
        _users = users;
        _tokens = tokens;
        _email = email;
    }

    public async Task Handle(ForgotPasswordRequest request, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(request.Email, ct);

        // Anti enumeration : toujours OK
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

        var link = $"https://taskboard/reset-password?token={token}";
        await _email.SendPasswordResetEmailAsync(user.Email, link);
    }
}
