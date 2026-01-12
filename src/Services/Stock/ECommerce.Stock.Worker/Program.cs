using ECommerce.Observability;
using ECommerce.Stock.Infrastructure.Data;
using ECommerce.Stock.Worker.Extensions;
using ECommerce.Stock.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using MediatR;


var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();

LoggingExtensions.ConfigureSerilog("StockService", builder.Environment);
builder.Services.AddSerilog();

builder.Services.AddCustomOpenTelemetry(builder.Configuration);
builder.Logging.AddCustomOpenTelemetry(builder.Configuration);
builder.Services.AddCustomMassTransit(builder.Configuration);
builder.Services.AddCustomDbContext(builder.Configuration);

builder.Services.AddScoped<ECommerce.Stock.Application.Abstractions.Persistence.IStockRepository, ECommerce.Stock.Infrastructure.Services.StockRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(ECommerce.Stock.Application.DependencyInjection).Assembly
));

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StockDbContext>();
    var attempts = 0;
    while (true)
    {
        try
        {
            db.Database.Migrate();
            break;
        }
        catch when (attempts < 5)
        {
            attempts++;
            Thread.Sleep(TimeSpan.FromSeconds(2));
        }
    }

    if (!db.StockItems.Any())
    {
        var now = DateTime.UtcNow;
        db.StockItems.AddRange(
            new ECommerce.Stock.Domain.StockItem { VariantId = 1, ProductId = 1, AvailableQuantity = 20, ReservedQuantity = 0, CreatedAt = now, MinimumQuantity = 0, IsActive = true },
            new ECommerce.Stock.Domain.StockItem { VariantId = 2, ProductId = 2, AvailableQuantity = 15, ReservedQuantity = 0, CreatedAt = now, MinimumQuantity = 0, IsActive = true },
            new ECommerce.Stock.Domain.StockItem { VariantId = 3, ProductId = 3, AvailableQuantity = 10, ReservedQuantity = 0, CreatedAt = now, MinimumQuantity = 0, IsActive = true },
            new ECommerce.Stock.Domain.StockItem { VariantId = 4, ProductId = 4, AvailableQuantity = 8, ReservedQuantity = 0, CreatedAt = now, MinimumQuantity = 0, IsActive = true },
            new ECommerce.Stock.Domain.StockItem { VariantId = 5, ProductId = 5, AvailableQuantity = 5, ReservedQuantity = 0, CreatedAt = now, MinimumQuantity = 0, IsActive = true });
        db.SaveChanges();
    }
}

Log.Information("Stock worker ready");

host.Run();
