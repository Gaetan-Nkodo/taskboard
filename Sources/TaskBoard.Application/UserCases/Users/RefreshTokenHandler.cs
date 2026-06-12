using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;
using TaskBoard.Application.Services;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.UseCases.Users;

public class RefreshTokenHandler
{
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUserRepository _users;
    private readonly ITokenService _tokens;
    private readonly IUnitOfWork _uow;

    public RefreshTokenHandler(
        IRefreshTokenRepository refreshTokens,
        IUserRepository users,
        ITokenService tokens,
        IUnitOfWork uow)
    {
        _refreshTokens = refreshTokens;
        _users = users;
        _tokens = tokens;
        _uow = uow;
    }

    public async Task<LoginResultDto> Handle(RefreshTokenRequest request, CancellationToken ct = default)
    {
        var stored = await _refreshTokens.GetByTokenAsync(request.RefreshToken, ct);
        if (stored is null)
            throw new InvalidCredentialsException();

        if (stored.IsExpired)
            throw new InvalidCredentialsException();

        if (stored.Revoked)
            throw new InvalidCredentialsException();

        var user = await _users.GetByIdAsync(stored.UserId, ct)
            ?? throw new InvalidCredentialsException();

        if (!user.IsActive)
            throw new AccountDisabledException();

        // Générer un nouveau refresh token
        var newRefreshValue = await _tokens.GenerateRefreshToken(user.Id);

        // Rotation du token existant (pas d'INSERT → pas de duplicate key)
        stored.Rotate(newRefreshValue, DateTime.UtcNow.AddDays(7));

        // Commit global
        await _uow.SaveChangesAsync(ct);

        var access = _tokens.GenerateToken(user.Id, user.Email);

        return new LoginResultDto(
            access,
            newRefreshValue,
            new UserDto(user.Id, user.Email, user.DisplayName)
        );
    }
}
