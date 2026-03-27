namespace TaskBoard.Application.Requests;

public record CreateTaskRequest(string Name, string? Description, string Icon, string Status);
