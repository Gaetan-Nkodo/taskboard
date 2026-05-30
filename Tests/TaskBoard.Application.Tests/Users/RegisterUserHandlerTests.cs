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

public class RegisterUserHandlerTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenDataIsValid()
    {
        var request = new RegisterUserRequest(
            Email: "new@example.com",
            Password: "P@ssw0rd!",
            DisplayName: "Gaetan"
        );

        _users.GetByEmailAsync(request.Email).Returns((User?)null);
        _hasher.Hash(request.Password).Returns("HASHED");

        var handler = new RegisterUserHandler(_users, _hasher);

        var result = await handler.Handle(request);

        result.Should().NotBeNull();
        result.Email.Should().Be("new@example.com");
        result.DisplayName.Should().Be("Gaetan");

        await _users.Received(1).AddAsync(Arg.Is<User>(u =>
            u.Email == "new@example.com" &&
            u.PasswordHash == "HASHED" &&
            u.DisplayName == "Gaetan"
        ));
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenEmailAlreadyExists()
    {
        var request = new RegisterUserRequest(
            Email: "exists@example.com",
            Password: "pwd",
            DisplayName: "Gaetan"
        );

        _users.GetByEmailAsync(request.Email)
              .Returns(new User("exists@example.com", "hash", "Existing"));

        var handler = new RegisterUserHandler(_users, _hasher);

        var act = () => handler.Handle(request);

        await act.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("Email already in use.");
    }
}
