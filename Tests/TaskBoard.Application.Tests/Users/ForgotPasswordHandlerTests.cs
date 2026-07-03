using FluentAssertions;

using Microsoft.Extensions.Configuration;

using NSubstitute;

using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Users;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Users;

public class ForgotPasswordHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordResetTokenRepository _tokens = Substitute.For<IPasswordResetTokenRepository>();
    private readonly IEmailSender _emailSender = Substitute.For<IEmailSender>();
    private readonly IConfiguration _config;

    public ForgotPasswordHandlerTests()
    {
        _config = Substitute.For<IConfiguration>();
        _config["Frontend:BaseUrl"].Returns("https://taskboard.app");
    }

    [Fact]
    public async Task Handle_ShouldSendResetEmail_WhenUserExists()
    {
        // Arrange
        var user = new User("test@example.com", "hash", "Gaetan");

        _users.GetByEmailAsync("test@example.com", default)
              .Returns(user);

        var handler = new ForgotPasswordHandler(
            _users,
            _tokens,
            _emailSender,
            _config
        );

        var request = new ForgotPasswordRequest("test@example.com");

        // Act
        await handler.Handle(request, default);

        // Assert
        await _tokens.Received(1).AddAsync(Arg.Any<PasswordResetToken>(), default);
        await _tokens.Received(1).SaveChangesAsync(default);

        await _emailSender.Received(1).SendAsync(
            "test@example.com",
            Arg.Any<string>(),
            Arg.Any<string>(),
            default
        );
    }

    [Fact]
    public async Task Handle_ShouldNotSendEmail_WhenUserDoesNotExist()
    {
        // Arrange
        _users.GetByEmailAsync("unknown@example.com", default)
              .Returns((User?)null);

        var handler = new ForgotPasswordHandler(
            _users,
            _tokens,
            _emailSender,
            _config
        );

        var request = new ForgotPasswordRequest("unknown@example.com");

        // Act
        await handler.Handle(request, default);

        // Assert
        await _tokens.DidNotReceive().AddAsync(Arg.Any<PasswordResetToken>(), default);
        await _emailSender.DidNotReceive().SendAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<string>(),
            default
        );
    }
}
