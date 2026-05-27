using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskBoard.Infrastructure.Persistence;
using TaskBoard.Domain.Entities;
using TaskBoard.Application.Services;

namespace TaskBoard.Api.Infrastructure.Database;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await context.Database.MigrateAsync();

        // Vérifie si un admin existe déjà
        if (!context.Users.Any(u => u.Email == "admin@taskboard.com"))
        {
            var admin = new User(
                email: "admin@taskboard.com",
                passwordHash: passwordHasher.Hash("Admin123!")
            );

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
