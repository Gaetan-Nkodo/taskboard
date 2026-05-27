using Microsoft.Extensions.DependencyInjection;

namespace TaskBoard.Api.Extensions;

public static class CorsExtensions
{
    private const string PolicyName = "AllowFrontend";

    public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration config)
    {
        var allowedOrigins = config.GetSection("Cors:AllowedOrigins").Get<string[]>()
                             ?? new[]
                             {
                                 "http://localhost:5173",
                                 "https://taskboard-frontend.onrender.com"
                             };

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IApplicationBuilder UseCustomCors(this IApplicationBuilder app)
    {
        app.UseCors(PolicyName);
        return app;
    }
}
