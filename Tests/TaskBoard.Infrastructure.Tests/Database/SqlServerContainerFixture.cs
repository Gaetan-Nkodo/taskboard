using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using TaskBoard.Infrastructure.Persistence;

namespace TaskBoard.IntegrationTests.Database;

public class SqlServerContainerFixture : IAsyncLifetime
{
    public MsSqlContainer Container { get; private set; } = default!;
    public AppDbContext Db { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        // Nouvelle API Testcontainers v3
        Container = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Your_password123")
            .Build();

        await Container.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(Container.GetConnectionString())
            .Options;

        Db = new AppDbContext(options);

        // Important : crée le schéma pour les tests
        await Db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.StopAsync();
        await Container.DisposeAsync();
    }
}
