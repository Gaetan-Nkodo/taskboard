using TaskBoard.Api.Middleware;
using TaskBoard.Api.Middlewares;

namespace TaskBoard.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<LoggingEnrichmentMiddleware>();
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseRouting();
        app.UseHttpsRedirection();

        return app;
    }
}
