using System.Text.RegularExpressions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using TaskBoard.Application.Common;
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
    private readonly ILogger<RegisterUserHandler> _logger;
    private readonly string _frontendUrl;

    public RegisterUserHandler(
        IUserRepository users,
        IPasswordHasher hasher,
        IEmailVerificationTokenRepository tokens,
        IEmailSender emailSender,
        IConfiguration config,
        ILogger<RegisterUserHandler> logger)
    {
        _users = users;
        _hasher = hasher;
        _tokens = tokens;
        _emailSender = emailSender;
        _logger = logger;

        _frontendUrl = FrontendUrlHelper.GetNormalizedFrontendUrl(config);
    }

    public async Task<UserDto> Handle(RegisterUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new DomainException("Email is required.");

        if (!IsValidEmail(request.Email))
            throw new DomainException("Invalid email format.");

        if (string.IsNullOrWhiteSpace(request.DisplayName))
            throw new DomainException("Display name is required.");

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

        try
        {
            await _emailSender.SendAsync(
                user.Email,
                "Confirme ton email",
                body
            );

            _logger.LogInformation(
                "Confirmation email sent to {Email} with token {Token}",
                user.Email,
                tokenValue
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send confirmation email to {Email}",
                user.Email
            );

            throw new DomainException("Failed to send confirmation email. Please try again later.");
        }

        return new UserDto(user.Id, user.Email, user.DisplayName);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
