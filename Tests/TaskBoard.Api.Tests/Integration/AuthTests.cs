using System.Text.Json;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using TaskBoard.Api.Controllers;
using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Users;
using TaskBoard.Domain.Exceptions;

namespace TaskBoard.Api.Tests;

public class AuthControllerTests
{
    private readonly ILoginUserHandler _login = Substitute.For<ILoginUserHandler>();
    private readonly IRegisterUserHandler _register = Substitute.For<IRegisterUserHandler>();

    [Fact]
    public async Task Login_ShouldReturn200_WithAccessAndRefreshTokens_AndUserDto()
    {
        var dto = new LoginResultDto(
            "ACCESS_TOKEN",
            "REFRESH_TOKEN",
            new UserDto(Guid.NewGuid(), "user@example.com", "Test User")
        );

        _login.Handle(Arg.Any<LoginUserRequest>())
              .Returns(dto);

        var controller = new AuthController(_register, _login);

        var result = await controller.Login(new LoginUserRequest("user@example.com", "pwd"))
                     as OkObjectResult;

        result.Should().NotBeNull();

        var json = JsonSerializer.Serialize(result!.Value);

        json.Should().Contain("ACCESS_TOKEN");
        json.Should().Contain("REFRESH_TOKEN");
        json.Should().Contain("user");
    }

    [Fact]
    public async Task Login_ShouldReturn401_WhenDomainExceptionThrown()
    {
        _login.Handle(Arg.Any<LoginUserRequest>())
              .ThrowsAsync(new InvalidCredentialsException());

        var controller = new AuthController(_register, _login);

        var result = await controller.Login(new LoginUserRequest("user@example.com", "pwd"))
                     as UnauthorizedObjectResult;

        result.Should().NotBeNull();

        var json = JsonSerializer.Serialize(result!.Value);

        json.Should().Contain("Invalid credentials");
    }
}
