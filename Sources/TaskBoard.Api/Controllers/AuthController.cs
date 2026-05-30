using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Users;
using TaskBoard.Domain.Exceptions;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IRegisterUserHandler _registerHandler;
    private readonly ILoginUserHandler _loginHandler;

    public AuthController(IRegisterUserHandler registerHandler, ILoginUserHandler loginHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var user = await _registerHandler.Handle(request);
        return Ok(new UserDto(user.Id, user.Email, user.DisplayName));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        try
        {
            var result = await _loginHandler.Handle(request);

            return Ok(new
            {
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken,
                user = result.User
            });
        }
        catch (InvalidCredentialsException)
        {
            return Unauthorized(new { error = "Invalid credentials" });
        }
        catch (AccountDisabledException)
        {
            return StatusCode(403, new { error = "Account disabled" });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
