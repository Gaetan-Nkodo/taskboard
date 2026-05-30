namespace TaskBoard.Application.DTOs;

public record LoginResultDto(string AccessToken, string RefreshToken, UserDto User);
