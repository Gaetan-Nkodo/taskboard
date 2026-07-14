using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

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

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();

            var apiDir = Path.GetDirectoryName(typeof(Program).Assembly.Location)!;

            config.AddJsonFile(Path.Combine(apiDir, "appsettings.json"), optional: false);
            config.AddJsonFile(Path.Combine(apiDir, "appsettings.Development.json"), optional: true);

            var testSettings = Path.Combine(apiDir, "appsettings.Test.json");
            if (File.Exists(testSettings))
                config.AddJsonFile(testSettings, optional: false);

            config.AddEnvironmentVariables();
        });

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

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

            // EMAIL SENDER → Fake
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
}
