using ECommerce.Observability;
using ECommerce.Stock.Api.Extensions;
using ECommerce.Stock.Application;
using ECommerce.Stock.Application.Abstractions.Persistence;
using ECommerce.Stock.Infrastructure.Data;
using ECommerce.Stock.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Polly;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

LoggingExtensions.ConfigureSerilog("StockApi", builder.Environment);
builder.Host.UseSerilog();

builder.Services.AddCustomDbContext(builder.Configuration);
builder.Services.AddCustomMassTransit(builder.Configuration);

builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddStockApplication();
builder.Services.AddObservabilityBehaviors();

builder.Services.AddCustomRedis(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomSwagger();

builder.Services.AddHealthChecks();

builder.Services.AddCustomOpenTelemetry(builder.Configuration);
builder.Logging.AddCustomOpenTelemetry(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Stock Service API v1");
    options.RoutePrefix = "swagger";
});

app.UseExceptionHandler();
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Service", "Stock-API");
    await next();
});

app.MapHealthChecks("/health");
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StockDbContext>();
    var retryPolicy = Policy.Handle<Exception>()
        .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(5));

    await retryPolicy.ExecuteAsync(async () =>
    {
        await db.Database.MigrateAsync();
    });
}

Log.Information("Stock API ready");

app.Run();
