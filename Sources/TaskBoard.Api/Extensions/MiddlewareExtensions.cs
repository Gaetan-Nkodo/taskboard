using TaskBoard.Api.Middleware;
using TaskBoard.Api.Middlewares;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<LoggingEnrichmentMiddleware>();
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseRouting();

        // Désactiver HTTPS en environnement de test
        var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();
        if (!env.IsEnvironment("Testing"))
        {
            app.UseHttpsRedirection();
        }

        return app;
    }
}
