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
    public async Task Should_Login_When_Credentials_Are_Valid()
    {
        // Arrange
        var user = new User("test@mail.com", "hashed-password");

        _users.GetByEmailAsync("test@mail.com").Returns(user);
        _hasher.Verify("password", "hashed-password").Returns(true);
        _tokens.GenerateToken(user.Id, user.Email).Returns("jwt-token");

        var handler = new LoginUserHandler(_users, _hasher, _tokens);

        var request = new LoginUserRequest("test@mail.com", "password");

        // Act
        var result = await handler.Handle(request);

        // Assert
        Assert.Equal(user.Id, result.User.Id);
        Assert.Equal("test@mail.com", result.User.Email);
        Assert.Equal("jwt-token", result.Token);
    }

    [Fact]
    public async Task Should_Throw_When_User_Does_Not_Exist()
    {
        // Arrange
        _users.GetByEmailAsync("unknown@mail.com").Returns((User?)null);

        var handler = new LoginUserHandler(_users, _hasher, _tokens);
        var request = new LoginUserRequest("unknown@mail.com", "password");

        // Act + Assert
        await Assert.ThrowsAsync<DomainException>(() => handler.Handle(request));
    }

    [Fact]
    public async Task Should_Throw_When_Password_Is_Invalid()
    {
        // Arrange
        var user = new User("test@mail.com", "hashed-password");

        _users.GetByEmailAsync("test@mail.com").Returns(user);
        _hasher.Verify("wrong-password", "hashed-password").Returns(false);

        var handler = new LoginUserHandler(_users, _hasher, _tokens);
        var request = new LoginUserRequest("test@mail.com", "wrong-password");

        // Act + Assert
        await Assert.ThrowsAsync<DomainException>(() => handler.Handle(request));
    }
}
