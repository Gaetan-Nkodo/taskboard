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
using TaskBoard.Infrastructure.Persistence;
using TaskBoard.Infrastructure.Persistence.Repositories;

namespace TaskBoard.Api.Tests.Fixtures;

public class ApiFactory : WebApplicationFactory<Program>
{
    private string? _connectionString;

    public void SetConnectionString(string connectionString)
    {
        _connectionString = connectionString;
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

            // Remplace DbContext
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(_connectionString),
                    contextLifetime: ServiceLifetime.Singleton,
                    optionsLifetime: ServiceLifetime.Singleton);

            // Désactive HTTPS obligatoire
            services.PostConfigure<Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionOptions>(o =>
            {
                o.HttpsPort = null;
            });

            // Remplace TokenService par FakeTokenService
            services.RemoveAll<ITokenService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<ITokenService, FakeTokenService>();

            // 🔥 Ajout des handlers nécessaires au AuthController
            services.AddScoped<RefreshTokenHandler>();
            services.AddScoped<LogoutUserHandler>();

            services.RemoveAll<IEmailService>();
            services.AddSingleton<IEmailService, FakeEmailService>();

            // MIGRATION AUTO
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
