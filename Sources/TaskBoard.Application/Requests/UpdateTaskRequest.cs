namespace TaskBoard.Application.Requests;

public record UpdateTaskRequest(string Name, string? Description, string Icon, string Status);
