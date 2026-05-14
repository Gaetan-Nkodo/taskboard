using Serilog;

namespace TaskBoard.Api.Extensions;

public static class LoggingExtensions
{
    public static void AddCustomSerilog(this IHostBuilder host)
    {
        host.UseSerilog((context, services, configuration) =>
        {
            configuration
                // Lis la config depuis appsettings.json (si tu veux ajouter Seq plus tard)
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)

                // Enrichissements standards
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .Enrich.WithCorrelationId()

                // Sinks (console + fichier)
                .WriteTo.Console()
                .WriteTo.File(
                    "logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7
                );
        });
    }
}
