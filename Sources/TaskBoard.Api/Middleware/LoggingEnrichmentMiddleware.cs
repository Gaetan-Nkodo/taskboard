using Serilog.Context;

namespace TaskBoard.Api.Middleware;

public class LoggingEnrichmentMiddleware
{
    private readonly RequestDelegate _next;

    public LoggingEnrichmentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.User.FindFirst("sub")?.Value;

        if (userId != null)
            LogContext.PushProperty("UserId", userId);

        // BoardId et TaskId détectés automatiquement dans l’URL
        var routeValues = context.Request.RouteValues;

        if (routeValues.TryGetValue("boardId", out var boardId))
            LogContext.PushProperty("BoardId", boardId);

        if (routeValues.TryGetValue("taskId", out var taskId))
            LogContext.PushProperty("TaskId", taskId);

        await _next(context);
    }
}
