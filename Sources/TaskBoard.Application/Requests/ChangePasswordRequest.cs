using MediatR;

namespace TaskBoard.Application.Requests;

public record ChangePasswordRequest(string CurrentPassword, string NewPassword) : IRequest;
