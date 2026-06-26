
namespace TaskBoard.Application.DTOs;

public record TaskDto(
    Guid Id,
    Guid ColumnId,
    string Name,
    string? Description,
    string? Icon,
    int Order
);
