using ECommerce.Messaging;
using ECommerce.Stock.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ECommerce.Stock.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransitWithRabbitMq(configuration);
        return services;
    }

    public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("StockDb")
            ?? configuration["STOCK_DB_CONNECTION"];

        services.AddDbContext<StockDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    public static IServiceCollection AddCustomRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis") 
                ?? configuration["Redis"] 
                ?? throw new InvalidOperationException("Redis configuration is missing");
        });

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
                        .AddService("StockService"))
                    .AddSource("MassTransit")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri($"{otelEndpoint}/v1/traces");
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    });
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService("StockService"))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri($"{otelEndpoint}/v1/metrics");
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

    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Stock Service API",
                Description = @"Stock availability and reservation management

**Features:**
- Query stock availability by product
- Bulk check stock for multiple products
- Real-time stock reservation via event-driven architecture",
                Contact = new OpenApiContact
                {
                    Name = "E-Commerce Support",
                    Email = "support@ecommerce.com"
                }
            });

            options.EnableAnnotations();
        });

        return services;
    }
}
