using FluentValidation;
using TaskBoard.Application.Services;
using TaskBoard.Application.UseCases.Boards;
using TaskBoard.Application.UseCases.Tasks;
using TaskBoard.Application.UseCases.Users;
using TaskBoard.Domain.Interfaces;
using TaskBoard.Infrastructure.Persistence.Repositories;
using TaskBoard.Infrastructure.Security;

namespace TaskBoard.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBoardRepository, BoardRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();

        // Security
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();

        // Handlers
        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<LoginUserHandler>();
        services.AddScoped<CreateBoardHandler>();
        services.AddScoped<GetBoardHandler>();
        services.AddScoped<GetBoardsByUserHandler>();
        services.AddScoped<UpdateBoardHandler>();
        services.AddScoped<DeleteBoardHandler>();
        services.AddScoped<CreateTaskHandler>();
        services.AddScoped<UpdateTaskHandler>();
        services.AddScoped<DeleteTaskHandler>();

        // Validators (un seul appel suffit)
        services.AddValidatorsFromAssembly(typeof(CreateBoardRequestValidator).Assembly);

        return services;
    }
}
