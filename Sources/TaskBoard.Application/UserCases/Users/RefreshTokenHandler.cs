using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;
using TaskBoard.Application.Services;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.UseCases.Users;

public class RefreshTokenHandler
{
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUserRepository _users;
    private readonly ITokenService _tokens;

    public RefreshTokenHandler(
        IRefreshTokenRepository refreshTokens,
        IUserRepository users,
        ITokenService tokens)
    {
        _refreshTokens = refreshTokens;
        _users = users;
        _tokens = tokens;
    }

    public async Task<LoginResultDto> Handle(RefreshTokenRequest request, CancellationToken ct = default)
    {
        var stored = await _refreshTokens.GetByTokenAsync(request.RefreshToken, ct);
        if (stored is null || stored.Revoked || stored.IsExpired)
            throw new InvalidCredentialsException();

        var user = await _users.GetByIdAsync(stored.UserId);
        if (user is null || !user.IsActive)
            throw new AccountDisabledException();

        stored.Revoke();
        var newRefreshToken = await _tokens.GenerateRefreshToken(user.Id);
        var accessToken = _tokens.GenerateToken(user.Id, user.Email);

        await _refreshTokens.SaveChangesAsync(ct);

        return new LoginResultDto(
            accessToken,
            newRefreshToken,
            new UserDto(user.Id, user.Email, user.DisplayName)
        );
    }
}
