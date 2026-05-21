namespace TaskBoard.Application.Requests;

public record CreateTaskRequest(
    Guid ColumnId,
    string Name,
    string? Description,
    string? Icon
);
