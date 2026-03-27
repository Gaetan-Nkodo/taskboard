namespace TaskBoard.Application.Requests;

public record UpdateBoardRequest(string Name, string? Description);