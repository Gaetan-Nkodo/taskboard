using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TaskBoard.Api.Health;

public class SeqHealthCheck : IHealthCheck
{
    private readonly HttpClient _client;

    public SeqHealthCheck()
    {
        _client = new HttpClient();
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.GetAsync("http://localhost:8081", cancellationToken);

            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy("Seq is reachable")
                : HealthCheckResult.Unhealthy("Seq returned an error");
        }
        catch
        {
            return HealthCheckResult.Unhealthy("Seq is unreachable");
        }
    }
}
