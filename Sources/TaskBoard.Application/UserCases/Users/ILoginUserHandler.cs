using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;

namespace TaskBoard.Application.UseCases.Users;

public interface ILoginUserHandler
{
    Task<LoginResultDto> Handle(LoginUserRequest request);
}
