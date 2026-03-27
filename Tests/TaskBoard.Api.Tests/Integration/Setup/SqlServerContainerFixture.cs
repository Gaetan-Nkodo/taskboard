using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using TaskBoard.Infrastructure.Persistence;

namespace TaskBoard.Api.Tests.Integration.Setup;

public class SqlServerContainerFixture : IAsyncLifetime
{
    public MsSqlContainer Container { get; private set; } = default!;
    public string ConnectionString => Container.GetConnectionString();

    public async Task InitializeAsync()
    {
        Container = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Your_password123")
            .Build();

        await Container.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.StopAsync();
        await Container.DisposeAsync();
    }
}
