using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TaskBoard.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=taskboard_local;Username=postgres;Password=postgres;SSL Mode=Disable",
            b => b.MigrationsAssembly("TaskBoard.Infrastructure")
        );

        return new AppDbContext(optionsBuilder.Options);
    }
}
