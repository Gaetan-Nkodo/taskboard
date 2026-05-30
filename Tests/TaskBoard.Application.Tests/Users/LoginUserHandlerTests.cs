using FluentAssertions;

using NSubstitute;

using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;
using TaskBoard.Application.Services;
using TaskBoard.Application.UseCases.Users;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;

namespace TaskBoard.Application.Tests.Users;

public class LoginUserHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokens = Substitute.For<ITokenService>();

    [Fact]
    public async Task Handle_ShouldReturnAccessAndRefreshTokens_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new LoginUserRequest("user@example.com", "P@ssw0rd!");

        var user = new User("user@example.com", "hashed", "Test User");

        _users.GetByEmailAsync(request.Email).Returns(user);
        _hasher.Verify(request.Password, user.PasswordHash).Returns(true);

        _tokens.GenerateToken(user.Id, user.Email).Returns("ACCESS_TOKEN");
        _tokens.GenerateRefreshToken(user.Id).Returns("REFRESH_TOKEN");

        var handler = new LoginUserHandler(_users, _hasher, _tokens);

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().Be("ACCESS_TOKEN");
        result.RefreshToken.Should().Be("REFRESH_TOKEN");

        result.User.Id.Should().Be(user.Id);
        result.User.Email.Should().Be("user@example.com");
        result.User.DisplayName.Should().Be("Test User");
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserNotFound()
    {
        var request = new LoginUserRequest("missing@example.com", "pwd");

        _users.GetByEmailAsync(request.Email).Returns((User?)null);

        var handler = new LoginUserHandler(_users, _hasher, _tokens);

        var act = () => handler.Handle(request);

        await act.Should()
            .ThrowAsync<InvalidCredentialsException>()
            .WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPasswordInvalid()
    {
        var request = new LoginUserRequest("user@example.com", "wrong");

        var user = new User("user@example.com", "hashed", "Test User");

        _users.GetByEmailAsync(request.Email).Returns(user);
        _hasher.Verify(request.Password, user.PasswordHash).Returns(false);

        var handler = new LoginUserHandler(_users, _hasher, _tokens);

        var act = () => handler.Handle(request);

        await act.Should()
            .ThrowAsync<InvalidCredentialsException>()
            .WithMessage("Invalid credentials.");
    }
}
