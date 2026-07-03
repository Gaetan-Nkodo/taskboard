using FluentAssertions;

using Microsoft.Extensions.Configuration;

using NSubstitute;

using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;
using TaskBoard.Application.Services;
using TaskBoard.Application.UseCases.Users;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Users;

public class RegisterUserHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly IEmailVerificationTokenRepository _tokens = Substitute.For<IEmailVerificationTokenRepository>();
    private readonly IEmailSender _emailSender = Substitute.For<IEmailSender>();
    private readonly IConfiguration _config;

    public RegisterUserHandlerTests()
    {
        _config = Substitute.For<IConfiguration>();
        _config["Frontend:BaseUrl"].Returns("https://taskboard.app");
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_AndSendEmail_WhenDataIsValid()
    {
        var request = new RegisterUserRequest(
            Email: "new@example.com",
            Password: "Abcd1234!",
            DisplayName: "Gaetan"
        );

        _users.GetByEmailAsync(request.Email).Returns((User?)null);
        _hasher.Hash(request.Password).Returns("HASHED");

        var handler = new RegisterUserHandler(_users, _hasher, _tokens, _emailSender, _config);

        var result = await handler.Handle(request);

        result.Email.Should().Be("new@example.com");

        await _users.Received(1).AddAsync(Arg.Any<User>());
        await _tokens.Received(1).AddAsync(Arg.Any<EmailVerificationToken>());
        await _emailSender.Received(1).SendAsync(
            "new@example.com",
            Arg.Any<string>(),
            Arg.Any<string>(),
            default
        );
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenEmailAlreadyExists()
    {
        var request = new RegisterUserRequest(
            Email: "exists@example.com",
            Password: "Abcd1234!",
            DisplayName: "Gaetan"
        );

        _users.GetByEmailAsync(request.Email)
              .Returns(new User("exists@example.com", "hash", "Existing"));

        var handler = new RegisterUserHandler(_users, _hasher, _tokens, _emailSender, _config);

        var act = () => handler.Handle(request);

        await act.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("Email already in use.");
    }
}
