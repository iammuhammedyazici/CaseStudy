using ECommerce.Messaging;
using ECommerce.Notification.Infrastructure.Consumers;
using MassTransit;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ECommerce.Notification.Worker.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransitWithRabbitMq(configuration, cfg =>
        {
            cfg.AddConsumer<StockReservedConsumer>();
            cfg.AddConsumer<StockFailedConsumer>();
        });
        return services;
    }

    public static IServiceCollection AddCustomOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var otelEndpoint = configuration["OpenTelemetry:Endpoint"]
            ?? throw new InvalidOperationException("OpenTelemetry:Endpoint configuration is missing");

        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService("NotificationWorker"))
                    .AddSource("MassTransit")
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri($"{otelEndpoint}/v1/traces");
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    });
            });

        return services;
    }

    public static ILoggingBuilder AddCustomOpenTelemetry(this ILoggingBuilder logging, IConfiguration configuration)
    {
        var otelEndpoint = configuration["OpenTelemetry:Endpoint"]
            ?? throw new InvalidOperationException("OpenTelemetry:Endpoint configuration is missing");

        logging.AddOpenTelemetry(options =>
        {
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
            options.AddOtlpExporter(exporter =>
            {
                exporter.Endpoint = new Uri($"{otelEndpoint}/v1/logs");
                exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
            });
        });

        return logging;
    }
}
