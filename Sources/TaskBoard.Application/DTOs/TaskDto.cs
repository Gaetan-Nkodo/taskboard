namespace TaskBoard.Application.DTOs;

public record TaskDto(Guid Id, string Name, string? Description, string? Icon, int Order);

