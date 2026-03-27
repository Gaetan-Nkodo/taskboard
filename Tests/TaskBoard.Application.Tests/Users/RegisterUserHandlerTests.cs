using NSubstitute;
using TaskBoard.Application.Requests;
using TaskBoard.Application.Services;
using TaskBoard.Application.UseCases.Users;
using TaskBoard.Domain.Entities;
using TaskBoard.Domain.Interfaces;

public class RegisterUserHandlerTests
{
    [Fact]
    public async Task Should_Register_User_When_Email_Not_Used()
    {
        var repo = Substitute.For<IUserRepository>();
        var hasher = Substitute.For<IPasswordHasher>();

        hasher.Hash("password").Returns("hashed");

        var handler = new RegisterUserHandler(repo, hasher);

        var result = await handler.Handle(new RegisterUserRequest("test@mail.com", "password"));

        Assert.Equal("test@mail.com", result.Email);
        await repo.Received(1).AddAsync(Arg.Any<User>());
    }
}
