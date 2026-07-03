using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using TaskBoard.Api.Tests.Utils;
using TaskBoard.Application.Services;
using TaskBoard.Application.UseCases.Users;
using TaskBoard.Domain.Interfaces;
using TaskBoard.Infrastructure.Persistance.Repositories;
using TaskBoard.Infrastructure.Persistence;
using TaskBoard.Infrastructure.Persistence.Repositories;
using TaskBoard.Infrastructure.Security;

namespace TaskBoard.Api.Tests.Fixtures;

public class ApiFactory : WebApplicationFactory<Program>
{
    private string? _connectionString;

    public void SetConnectionString(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"RefreshTokens\" RESTART IDENTITY CASCADE;");
        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"PasswordResetTokens\" RESTART IDENTITY CASCADE;");
        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Users\" RESTART IDENTITY CASCADE;");
        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Boards\" RESTART IDENTITY CASCADE;");
        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Columns\" RESTART IDENTITY CASCADE;");
        await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Tasks\" RESTART IDENTITY CASCADE;");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var solutionRoot = FindSolutionRoot(Directory.GetCurrentDirectory());
            var apiDir = FindProjectDirectory(solutionRoot, "TaskBoard.Api");

            config.AddJsonFile(Path.Combine(apiDir, "appsettings.json"), optional: false);
            config.AddJsonFile(Path.Combine(apiDir, "appsettings.Development.json"), optional: true);

            var testSettings = Path.Combine(apiDir, "appsettings.Test.json");
            if (File.Exists(testSettings))
                config.AddJsonFile(testSettings, optional: false);
        });

        builder.ConfigureServices(services =>
        {
            if (_connectionString == null)
                throw new InvalidOperationException("Connection string not set.");

            // DB
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_connectionString));

            // HTTPS OFF
            services.PostConfigure<Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionOptions>(o =>
            {
                o.HttpsPort = null;
            });

            // TOKEN SERVICE
            services.RemoveAll<ITokenService>();
            services.AddScoped<ITokenService, JwtTokenService>();

            // PASSWORD HASHER FAKE
            services.RemoveAll<IPasswordHasher>();
            services.AddSingleton<IPasswordHasher, FakePasswordHasher>();

            // REPOSITORIES
            services.RemoveAll<IUserRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.RemoveAll<IRefreshTokenRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.RemoveAll<IPasswordResetTokenRepository>();
            services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();

            services.RemoveAll<IEmailVerificationTokenRepository>();
            services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();

            // UNIT OF WORK
            services.RemoveAll<IUnitOfWork>();
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            // EMAIL SENDER → remplacer Mailjet par FakeEmailSender
            services.RemoveAll<IEmailSender>();
            services.AddSingleton<IEmailSender, FakeEmailSender>();

            // HANDLERS
            services.RemoveAll<IRegisterUserHandler>();
            services.AddScoped<IRegisterUserHandler, RegisterUserHandler>();

            services.RemoveAll<ILoginUserHandler>();
            services.AddScoped<ILoginUserHandler>(sp =>
                new LoginUserHandler(
                    sp.GetRequiredService<IUserRepository>(),
                    sp.GetRequiredService<IPasswordHasher>(),
                    sp.GetRequiredService<ITokenService>(),
                    sp.GetRequiredService<IRefreshTokenRepository>(),
                    sp.GetRequiredService<IUnitOfWork>()
                )
            );

            services.RemoveAll<RefreshTokenHandler>();
            services.AddScoped<RefreshTokenHandler>();

            services.RemoveAll<LogoutUserHandler>();
            services.AddScoped<LogoutUserHandler>();

            services.RemoveAll<ForgotPasswordHandler>();
            services.AddScoped<ForgotPasswordHandler>();

            services.RemoveAll<ResetPasswordHandler>();
            services.AddScoped<ResetPasswordHandler>();

            services.RemoveAll<ChangePasswordHandler>();
            services.AddScoped<ChangePasswordHandler>();

            // MIGRATIONS
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        });
    }

    private static string FindSolutionRoot(string startDir)
    {
        var dir = new DirectoryInfo(startDir);

        while (dir != null)
        {
            if (dir.GetFiles("*.sln").Any())
                return dir.FullName;

            dir = dir.Parent;
        }

        throw new Exception("Impossible de trouver la racine de la solution (.sln).");
    }

    private static string FindProjectDirectory(string solutionDir, string projectName)
    {
        var projectFile = Directory.GetFiles(solutionDir, $"{projectName}.csproj", SearchOption.AllDirectories)
                                   .FirstOrDefault();

        if (projectFile == null)
            throw new Exception($"Impossible de trouver {projectName}.csproj dans la solution.");

        return Path.GetDirectoryName(projectFile)!;
    }
}
