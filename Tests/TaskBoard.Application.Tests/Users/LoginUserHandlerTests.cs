using FluentAssertions;
using NSubstitute;
using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Users;
using TaskBoard.Application.Services;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;
using Xunit;

namespace TaskBoard.Application.Tests.Users;

public class LoginUserHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokens = Substitute.For<ITokenService>();

    [Fact]
    public async Task Handle_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new LoginUserRequest("user@example.com", "P@ssw0rd!");

        var user = new User("user@example.com", "hashed");

        _users.GetByEmailAsync(request.Email).Returns(user);
        _hasher.Verify(request.Password, user.PasswordHash).Returns(true);
        _tokens.GenerateToken(user.Id, user.Email).Returns("jwt-token");

        var handler = new LoginUserHandler(_users, _hasher, _tokens);

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().BeOfType<LoginResultDto>();
        result.Token.Should().Be("jwt-token");
        result.User.Email.Should().Be("user@example.com");
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenUserNotFound()
    {
        // Arrange
        var request = new LoginUserRequest("user@example.com", "P@ssw0rd!");

        _users.GetByEmailAsync(request.Email).Returns((User?)null);

        var handler = new LoginUserHandler(_users, _hasher, _tokens);

        // Act
        var act = () => handler.Handle(request);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("User Not Found.");
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenPasswordInvalid()
    {
        // Arrange
        var request = new LoginUserRequest("user@example.com", "wrong");

        var user = new User("user@example.com", "hashed");

        _users.GetByEmailAsync(request.Email).Returns(user);
        _hasher.Verify(request.Password, user.PasswordHash).Returns(false);

        var handler = new LoginUserHandler(_users, _hasher, _tokens);

        // Act
        var act = () => handler.Handle(request);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Invalid credentials.");
    }
}
