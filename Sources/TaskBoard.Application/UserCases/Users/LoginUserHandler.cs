using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;
using TaskBoard.Application.Services;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.UseCases.Users;

public class LoginUserHandler : ILoginUserHandler
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUnitOfWork _uow;

    public LoginUserHandler(
        IUserRepository users,
        IPasswordHasher hasher,
        ITokenService tokens,
        IRefreshTokenRepository refreshTokens,
        IUnitOfWork uow)
    {
        _users = users;
        _hasher = hasher;
        _tokens = tokens;
        _refreshTokens = refreshTokens;
        _uow = uow;
    }

    public async Task<LoginResultDto> Handle(LoginUserRequest request)
    {
        var user = await _users.GetByEmailAsync(request.Email);
        if (user == null)
            throw new InvalidCredentialsException();

        if (!user.EmailConfirmed)
            throw new EmailNotConfirmedException();

        if (!_hasher.Verify(request.Password, user.PasswordHash))
            throw new InvalidCredentialsException();

        if (!user.IsActive)
            throw new AccountDisabledException();

        // 1. Générer l'access token
        var accessToken = _tokens.GenerateToken(user.Id, user.Email);

        // 2. Générer la valeur du refresh token
        var refreshTokenValue = await _tokens.GenerateRefreshToken(user.Id);

        // 3. Créer et stocker l'entité RefreshToken
        var refreshEntity = new RefreshToken(
            user.Id,
            refreshTokenValue,
            DateTime.UtcNow.AddDays(7)
        );

        await _refreshTokens.StoreAsync(refreshEntity);

        // 4. Commit global
        await _uow.SaveChangesAsync();

        // 5. Retourner le résultat
        return new LoginResultDto(
            accessToken,
            refreshTokenValue,
            new UserDto(user.Id, user.Email, user.DisplayName)
        );
    }
}
