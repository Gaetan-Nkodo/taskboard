using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.UserCases.Users;

public class ConfirmEmailHandler
{
    private readonly IEmailVerificationTokenRepository _tokens;
    private readonly IUserRepository _users;

    public ConfirmEmailHandler(IEmailVerificationTokenRepository tokens, IUserRepository users)
    {
        _tokens = tokens;
        _users = users;
    }

    public async Task Handle(string tokenValue, CancellationToken ct)
    {
        var token = await _tokens.GetByTokenAsync(tokenValue, ct);
        if (token == null || token.IsExpired())
            throw new DomainException("Invalid or expired token.");

        var user = await _users.GetByIdAsync(token.UserId, ct);
        if (user == null)
            throw new DomainException("User not found.");

        user.ConfirmEmail();
        await _users.SaveChangesAsync(ct);
        await _tokens.RemoveAsync(token, ct);
    }
}
