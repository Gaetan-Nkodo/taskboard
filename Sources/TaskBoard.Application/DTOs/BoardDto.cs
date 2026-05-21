namespace TaskBoard.Application.DTOs;

public record BoardDto(Guid Id, string Name, string? Description, List<ColumnDto> Columns);
