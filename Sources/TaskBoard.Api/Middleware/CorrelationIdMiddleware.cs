using System.Diagnostics;

namespace TaskBoard.Api.Middlewares;

public class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-ID";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Si le client en fournit un, on le garde
        if (!context.Request.Headers.TryGetValue(HeaderName, out var correlationId))
        {
            correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString();
            context.Request.Headers[HeaderName] = correlationId;
        }

        // On le met dans la réponse
        context.Response.Headers[HeaderName] = correlationId;

        // On le met dans le LogContext
        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId.ToString()))
        {
            await _next(context);
        }
    }
}
