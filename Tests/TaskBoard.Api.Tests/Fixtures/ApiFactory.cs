using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            var solutionRoot = FindSolutionRoot(Directory.GetCurrentDirectory());
            var apiProjectDir = FindProjectDirectory(solutionRoot, "TaskBoard.Api");

            configBuilder.AddJsonFile(Path.Combine(apiProjectDir, "appsettings.json"), optional: false);
            configBuilder.AddJsonFile(Path.Combine(apiProjectDir, "appsettings.Development.json"), optional: true);
        });

        builder.ConfigureServices(services =>
        {
            if (_connectionString == null)
                throw new InvalidOperationException("Connection string not set.");

            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_connectionString));

            services.PostConfigure<HttpsRedirectionOptions>(options =>
            {
                options.HttpsPort = null;
            });
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
