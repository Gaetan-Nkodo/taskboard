using FluentAssertions;
using NSubstitute;
using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Users;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Exceptions;
using TaskBoard.Domain.Interfaces;
using TaskBoard.Application.Services;
using Xunit;

namespace TaskBoard.Application.Tests.Users;

public class RegisterUserHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();

    [Fact]
    public async Task Handle_ShouldRegisterUser_WhenEmailNotUsed()
    {
        // Arrange
        var request = new RegisterUserRequest("user@example.com", "P@ssw0rd!");

        _users.GetByEmailAsync(request.Email).Returns((User?)null);
        _hasher.Hash(request.Password).Returns("hashed");

        var handler = new RegisterUserHandler(_users, _hasher);

        // Act
        var result = await handler.Handle(request);

        // Assert
        result.Should().BeOfType<UserDto>();
        result.Email.Should().Be("user@example.com");

        await _users.Received(1).AddAsync(Arg.Is<User>(u =>
            u.Email == "user@example.com" &&
            u.PasswordHash == "hashed"
        ));
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenEmailAlreadyUsed()
    {
        // Arrange
        var request = new RegisterUserRequest("user@example.com", "P@ssw0rd!");

        _users.GetByEmailAsync(request.Email).Returns(new User("user@example.com", "hash"));

        var handler = new RegisterUserHandler(_users, _hasher);

        // Act
        var act = () => handler.Handle(request);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Email already in use.");
    }
}
