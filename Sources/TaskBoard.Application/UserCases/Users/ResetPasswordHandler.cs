using MediatR;

using TaskBoard.Application.Requests;
using TaskBoard.Application.Services;
using TaskBoard.Domain.Interfaces;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordRequest>
{
    private readonly IPasswordResetTokenRepository _tokens;
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IPasswordHasher _hasher;

    public ResetPasswordHandler(
        IPasswordResetTokenRepository tokens,
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        IPasswordHasher hasher)
    {
        _tokens = tokens;
        _users = users;
        _refreshTokens = refreshTokens;
        _hasher = hasher;
    }

    public async Task Handle(ResetPasswordRequest request, CancellationToken ct)
    {
        var token = await _tokens.GetByTokenAsync(request.Token, ct);

        if (token is null || !token.IsValid())
            throw new UnauthorizedAccessException("Invalid or expired token");

        var user = await _users.GetByIdAsync(token.UserId, ct);
        if (user is null)
            throw new UnauthorizedAccessException("Invalid token");

        // Mise à jour du mot de passe
        user.UpdatePassword(_hasher.Hash(request.NewPassword));

        // Marquer le token comme utilisé
        token.MarkUsed();

        // Révoquer tous les refresh tokens
        await _refreshTokens.RevokeAllForUserAsync(user.Id, ct);

        // 🔥 Sauvegarde CLEAN ARCHITECTURE
        await _users.SaveChangesAsync(ct);
        await _tokens.SaveChangesAsync(ct);
        await _refreshTokens.SaveChangesAsync(ct);
    }
}
