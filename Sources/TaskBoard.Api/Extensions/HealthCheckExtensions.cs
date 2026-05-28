using Microsoft.Extensions.Diagnostics.HealthChecks;

using TaskBoard.Api.Health;

namespace TaskBoard.Api.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddCheck<SeqHealthCheck>("seq")
            .AddCheck("api", () => HealthCheckResult.Healthy("API OK"));

        return services;
    }
}
