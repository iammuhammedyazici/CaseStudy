using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.OpenTelemetry;
using System;

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

        var otlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        if (!string.IsNullOrWhiteSpace(otlpEndpoint))
        {
            var endpoint = otlpEndpoint.TrimEnd('/');
            if (!endpoint.EndsWith("/v1/logs", StringComparison.OrdinalIgnoreCase))
            {
                endpoint = $"{endpoint}/v1/logs";
            }

            configuration.WriteTo.OpenTelemetry(options =>
            {
                options.Endpoint = endpoint;
                options.Protocol = OtlpProtocol.HttpProtobuf;
            });
        }

        Log.Logger = configuration.CreateLogger();
    }
}
