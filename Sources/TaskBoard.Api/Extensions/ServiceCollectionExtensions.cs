using FluentValidation;

using TaskBoard.Application.Requests;
using TaskBoard.Application.Services;
using TaskBoard.Application.UseCases.Boards;
using TaskBoard.Application.UseCases.Tasks;
using TaskBoard.Application.UseCases.Users;
using TaskBoard.Domain.Interfaces;
using TaskBoard.Infrastructure.Persistance.Repositories;
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
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();

        // Security
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IEmailSender, MailjetEmailSender>();

        // Handlers
        services.AddScoped<IRegisterUserHandler, RegisterUserHandler>();
        services.AddScoped<ILoginUserHandler, LoginUserHandler>();
        services.AddScoped<RefreshTokenHandler>();
        services.AddScoped<LogoutUserHandler>();
        services.AddScoped<CreateBoardHandler>();
        services.AddScoped<GetBoardHandler>();
        services.AddScoped<GetBoardsByUserHandler>();
        services.AddScoped<UpdateBoardHandler>();
        services.AddScoped<DeleteBoardHandler>();
        services.AddScoped<CreateTaskHandler>();
        services.AddScoped<UpdateTaskHandler>();
        services.AddScoped<DeleteTaskHandler>();
        services.AddScoped<ForgotPasswordHandler>();
        services.AddScoped<ResetPasswordHandler>();
        services.AddScoped<ChangePasswordHandler>();
        services.AddScoped<GetTasksByBoardHandler>();
        services.AddScoped<MoveTaskHandler>();

        // Validators
        services.AddValidatorsFromAssembly(typeof(CreateBoardRequestValidator).Assembly);

        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ForgotPasswordRequest).Assembly));

        return services;
    }
}
