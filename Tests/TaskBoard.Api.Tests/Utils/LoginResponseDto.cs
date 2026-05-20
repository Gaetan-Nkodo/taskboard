public sealed record LoginResponseDto(string Token, UserDto User);

public sealed record UserDto(Guid Id, string Email);
