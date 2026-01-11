using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace ECommerce.Observability;

public static class LoggingExtensions
{
    public static void ConfigureSerilog(string serviceName, IHostEnvironment environment)
    {
        var minimumLevel = environment.IsProduction()
            ? LogEventLevel.Warning
            : LogEventLevel.Information;

        var configuration = new LoggerConfiguration()
            .MinimumLevel.Is(minimumLevel)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ServiceName", serviceName)
            .Enrich.WithProperty("Environment", environment.EnvironmentName)
            .Enrich.WithClientIp();

        if (environment.IsProduction())
        {
            configuration.WriteTo.Console(new CompactJsonFormatter());
        }
        else
        {
            configuration.WriteTo.Console();
        }

        Log.Logger = configuration.CreateLogger();
    }
}
