using ECommerce.Product.Infrastructure.Data;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ECommerce.Product.Api.Extensions;

public static class ServiceCollectionExtensions
{
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
                        .AddService("ProductService"))
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
                        .AddService("ProductService"))
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
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product Service API", Version = "v1" });
        });

        return services;
    }
}
