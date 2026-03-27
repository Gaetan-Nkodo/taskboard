namespace TaskBoard.Application.DTOs;

public record LoginResultDto(UserDto User, string Token);
