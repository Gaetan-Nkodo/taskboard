using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TaskBoard.Api.Extensions;
using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Users;
using TaskBoard.Application.UserCases.Users;
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
    private readonly IMediator _mediator;

    public AuthController(
        IRegisterUserHandler registerHandler,
        ILoginUserHandler loginHandler,
        RefreshTokenHandler refreshHandler,
        LogoutUserHandler logoutHandler,
        IMediator mediator)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
        _refreshHandler = refreshHandler;
        _logoutHandler = logoutHandler;
        _mediator = mediator;
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
        catch (InvalidCredentialsException)
        {
            return Unauthorized(new { error = "Invalid refresh token" });
        }
        catch (AccountDisabledException)
        {
            return Unauthorized(new { error = "Account disabled" });
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

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _mediator.Send(request);
        return Ok(new { message = "If this email exists, a reset link has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            await _mediator.Send(request);
            return Ok(new { message = "Password updated successfully." });
        }
        catch (InvalidCredentialsException)
        {
            return Unauthorized(new { error = "Invalid or expired token" });
        }
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var handler = HttpContext.RequestServices.GetRequiredService<ChangePasswordHandler>();
        handler.UserId = User.GetUserId();

        try
        {
            await handler.Handle(request, HttpContext.RequestAborted);
            return Ok(new { message = "Password updated" });
        }
        catch (InvalidCredentialsException)
        {
            return Unauthorized(new { error = "Invalid password" });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, CancellationToken ct)
    {
        var handler = HttpContext.RequestServices.GetRequiredService<ConfirmEmailHandler>();
        await handler.Handle(token, ct);
        return Ok(new { message = "Email confirmed" });
    }

}
