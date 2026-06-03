using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.UseCases.Users;

public class LogoutUserHandler
{
    private readonly IRefreshTokenRepository _refreshTokens;

    public LogoutUserHandler(IRefreshTokenRepository refreshTokens)
    {
        _refreshTokens = refreshTokens;
    }

    public async Task Handle(Guid userId, CancellationToken ct = default)
    {
        await _refreshTokens.RevokeAllForUserAsync(userId, ct);
        await _refreshTokens.SaveChangesAsync(ct);
    }
}
