using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;
using TaskBoard.Application.Services;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;
using TaskBoard.Domain.Exceptions;

namespace TaskBoard.Application.UseCases.Users;

public class RegisterUserHandler
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;

    public RegisterUserHandler(IUserRepository users, IPasswordHasher hasher)
    {
        _users = users;
        _hasher = hasher;
    }

    public async Task<UserDto> Handle(RegisterUserRequest request)
    {
        var existing = await _users.GetByEmailAsync(request.Email);
        if (existing != null)
            throw new DomainException("Email already in use.");

        var hash = _hasher.Hash(request.Password);
        var user = new User(request.Email, hash);

        await _users.AddAsync(user);

        return new UserDto(user.Id, user.Email);
    }
}
