using Microsoft.EntityFrameworkCore;

namespace TaskBoard.Api.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddCustomDatabase(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(
                config.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("TaskBoard.Infrastructure")
            );
        });

        return services;
    }
}
