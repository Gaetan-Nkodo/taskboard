using Microsoft.AspNetCore.Mvc;
using TaskBoard.Application.Requests;
using TaskBoard.Application.UseCases.Users;
using TaskBoard.Domain.Exceptions;

namespace TaskBoard.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly RegisterUserHandler _registerHandler;
    private readonly LoginUserHandler _loginHandler;

    public AuthController(RegisterUserHandler registerHandler, LoginUserHandler loginHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var userId = await _registerHandler.Handle(request);
        return CreatedAtAction(nameof(Register), new { id = userId }, new { id = userId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var token = await _loginHandler.Handle(request);
        return Ok(new { token });
    }
}
