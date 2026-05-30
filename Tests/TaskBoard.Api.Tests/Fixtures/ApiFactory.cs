using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using TaskBoard.Api.Tests.Utils;
using TaskBoard.Application.Services;
using TaskBoard.Infrastructure.Persistence;

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

            // 🔥 Ajout d’un fichier de config spécial tests
            var testSettings = Path.Combine(apiDir, "appsettings.Test.json");
            if (File.Exists(testSettings))
                config.AddJsonFile(testSettings, optional: false);
        });

        builder.ConfigureServices(services =>
        {
            if (_connectionString == null)
                throw new InvalidOperationException("Connection string not set.");

            // 🔥 Remplace le DbContext
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_connectionString));

            // 🔥 Désactive HTTPS obligatoire
            services.PostConfigure<Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionOptions>(o =>
            {
                o.HttpsPort = null;
            });

            // 🔥 Remplace le TokenService par FakeTokenService
            services.RemoveAll<ITokenService>();
            services.AddSingleton<ITokenService, FakeTokenService>();

            // 🔥🔥🔥 MIGRATION AUTOMATIQUE POUR LES TESTS 🔥🔥🔥
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
