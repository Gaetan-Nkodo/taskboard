using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TaskBoard.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=localhost;Database=TaskBoard;Trusted_Connection=True;TrustServerCertificate=True;",
            b => b.MigrationsAssembly("TaskBoard.Infrastructure")
        );

        return new AppDbContext(optionsBuilder.Options);
    }
}
