using TaskBoard.Application.DTOs;

public record ColumnDto(Guid Id, string Name, int Order, List<TaskDto> Tasks);