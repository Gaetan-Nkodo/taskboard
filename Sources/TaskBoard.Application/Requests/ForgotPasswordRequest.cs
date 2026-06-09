using MediatR;

namespace TaskBoard.Application.Requests;

public record ForgotPasswordRequest(string Email) : IRequest;
