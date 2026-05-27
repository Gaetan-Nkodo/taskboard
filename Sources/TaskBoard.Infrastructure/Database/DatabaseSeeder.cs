using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using TaskBoard.Application.Services;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Persistence;

namespace TaskBoard.Api.Infrastructure.Database;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        try
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            await context.Database.MigrateAsync();

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
        catch (Exception ex)
        {
            Console.WriteLine("SEED ERROR: " + ex.Message);
        }
    }
}
