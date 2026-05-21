using Microsoft.EntityFrameworkCore;

using Testcontainers.MsSql;

namespace TaskBoard.Api.Tests.Fixtures;

public class SqlServerContainerFixture : IAsyncLifetime
{
    public MsSqlContainer Container { get; }
    public string ConnectionString => Container.GetConnectionString();

    public SqlServerContainerFixture()
    {
        Container = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Your_password123")
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithEnvironment("MSSQL_PID", "Developer")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await Container.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(ConnectionString)
            .Options;

        using var db = new AppDbContext(options);
        await db.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.DisposeAsync();
    }
}
