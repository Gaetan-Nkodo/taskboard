using Microsoft.Extensions.Configuration;

using TaskBoard.Application.Common.Emails;
using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;
using TaskBoard.Application.Services;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.UseCases.Users;

public class RegisterUserHandler : IRegisterUserHandler
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IEmailVerificationTokenRepository _tokens;
    private readonly IEmailSender _emailSender;
    private readonly string _frontendUrl;

    public RegisterUserHandler(
        IUserRepository users,
        IPasswordHasher hasher,
        IEmailVerificationTokenRepository tokens,
        IEmailSender emailSender,
        IConfiguration config)
    {
        _users = users;
        _hasher = hasher;
        _tokens = tokens;
        _emailSender = emailSender;

        _frontendUrl = config["Frontend:BaseUrl"] ?? "https://taskboard.app";
    }

    public async Task<UserDto> Handle(RegisterUserRequest request)
    {
        var existing = await _users.GetByEmailAsync(request.Email);
        if (existing != null)
            throw new DomainException("Email already in use.");

        var hash = _hasher.Hash(request.Password);
        var user = new User(request.Email, hash, request.DisplayName);

        await _users.AddAsync(user);

        var tokenValue = Guid.NewGuid().ToString("N");
        var token = new EmailVerificationToken(
            user.Id,
            tokenValue,
            DateTime.UtcNow.AddHours(24)
        );

        await _tokens.AddAsync(token);

        var confirmUrl = $"{_frontendUrl}/confirm-email?token={tokenValue}";
        var body = EmailTemplates.ConfirmEmail(user.DisplayName, confirmUrl);

        await _emailSender.SendAsync(
            user.Email,
            "Confirme ton email",
            body
        );

        return new UserDto(user.Id, user.Email, user.DisplayName);
    }
}
