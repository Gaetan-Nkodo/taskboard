using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using TaskBoard.Infrastructure.Persistence;

namespace TaskBoard.Api.Tests.Integration.Setup;

public class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public ApiFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext
            var descriptor = services.Single(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            services.Remove(descriptor);

            // Replace with Testcontainers SQL Server
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(_connectionString));
        });
    }
}
