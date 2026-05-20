namespace TaskBoard.Application.DTOs;

public record UserDto(Guid Id, string Email);

public sealed record LoginResponseDto(string Token, UserDto User);
