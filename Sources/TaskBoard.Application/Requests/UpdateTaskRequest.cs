namespace TaskBoard.Application.Requests;

public record UpdateTaskRequest(
    Guid ColumnId,
    string Name,
    string? Description,
    string? Icon
);
