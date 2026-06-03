using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TaskBoard.Api.Extensions;
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
    private readonly RefreshTokenHandler _refreshHandler;
    private readonly LogoutUserHandler _logoutHandler;

    public AuthController(
        IRegisterUserHandler registerHandler,
        ILoginUserHandler loginHandler,
        RefreshTokenHandler refreshHandler,
        LogoutUserHandler logoutHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
        _refreshHandler = refreshHandler;
        _logoutHandler = logoutHandler;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterUserRequest request)
    {
        var user = await _registerHandler.Handle(request);
        return Ok(new UserDto(user.Id, user.Email, user.DisplayName));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResultDto>> Login([FromBody] LoginUserRequest request)
    {
        try
        {
            var result = await _loginHandler.Handle(request);
            return Ok(result);
        }
        catch (InvalidCredentialsException)
        {
            return Unauthorized(new { error = "Invalid credentials" });
        }
        catch (AccountDisabledException)
        {
            return StatusCode(403, new { error = "Account disabled" });
        }
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResultDto>> Refresh([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var result = await _refreshHandler.Handle(request);
            return Ok(result);
        }
        catch
        {
            return Unauthorized(new { error = "Invalid refresh token" });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var userId = User.GetUserId();
        await _logoutHandler.Handle(userId, ct);
        return Ok(new { message = "Logged out" });
    }
}
