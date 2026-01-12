using ECommerce.Messaging;
using ECommerce.Stock.Infrastructure.Consumers;
using ECommerce.Stock.Infrastructure.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ECommerce.Stock.Worker.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransitWithRabbitMq(configuration, cfg =>
        {
            cfg.AddConsumer<OrderCreatedConsumer>();
            cfg.AddConsumer<CheckStockConsumer>();
        });
        return services;
    }

    public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("StockDb")
            ?? configuration["STOCK_DB_CONNECTION"]
            ?? configuration["ConnectionStrings__StockDb"];

        services.AddDbContext<StockDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    public static IServiceCollection AddCustomOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var otelEndpoint = configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]
            ?? configuration["OpenTelemetry:Endpoint"]
            ?? throw new InvalidOperationException("OpenTelemetry:Endpoint configuration is missing");

        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService("StockWorker"))
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
        var otelEndpoint = configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]
            ?? configuration["OpenTelemetry:Endpoint"]
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
