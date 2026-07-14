namespace TaskBoard.Api.Extensions;

public static class ConfigurationExtensions
{
    public static void AddCustomConfiguration(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsEnvironment("Testing"))
            return;

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddJsonFile("serilog.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    }
}
