using MediatR;

namespace TaskBoard.Application.Requests;

public record ResetPasswordRequest(string Token, string NewPassword) : IRequest;
