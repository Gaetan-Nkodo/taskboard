using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using TaskBoard.Infrastructure.Persistence;
using Xunit;

namespace TaskBoard.IntegrationTests.Database;

public class SqlServerContainerFixture : IAsyncLifetime
{
    public MsSqlContainer Container { get; private set; } = default!;
    public AppDbContext Db { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        // API Testcontainers v3 — version propre et non obsolète
        Container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
                        .WithPassword("Your_password123")
                        .WithEnvironment("ACCEPT_EULA", "Y")
                        .WithEnvironment("MSSQL_PID", "Developer")
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
        if (Db is not null)
            await Db.DisposeAsync();

        if (Container is not null)
        {
            await Container.StopAsync();
            await Container.DisposeAsync();
        }
    }
}
