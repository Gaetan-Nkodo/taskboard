using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;
using TaskBoard.Application.Services;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.UseCases.Users;

public class LoginUserHandler
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;

    public LoginUserHandler(IUserRepository users, IPasswordHasher hasher, ITokenService tokens)
    {
        _users = users;
        _hasher = hasher;
        _tokens = tokens;
    }

    public async Task<LoginResultDto> Handle(LoginUserRequest request)
    {
        var user = await _users.GetByEmailAsync(request.Email);
        if (user == null)
            throw new DomainException("User Not Found.");

        if (!_hasher.Verify(request.Password, user.PasswordHash))
            throw new DomainException("Invalid credentials.");

        var token = _tokens.GenerateToken(user.Id, user.Email);

        return new LoginResultDto(new UserDto(user.Id, user.Email), token);
    }
}
