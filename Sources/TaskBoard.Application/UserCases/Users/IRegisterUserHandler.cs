using TaskBoard.Application.DTOs;
using TaskBoard.Application.Requests;

namespace TaskBoard.Application.UseCases.Users;

public interface IRegisterUserHandler
{
    Task<UserDto> Handle(RegisterUserRequest request);
}
