using ECommerce.Observability;
using ECommerce.Stock.Infrastructure.Data;
using ECommerce.Stock.Worker.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;


var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();

LoggingExtensions.ConfigureSerilog("StockService", builder.Environment);
builder.Services.AddSerilog();

builder.Services.AddCustomOpenTelemetry(builder.Configuration);
builder.Logging.AddCustomOpenTelemetry(builder.Configuration);
builder.Services.AddCustomMassTransit(builder.Configuration);

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
        db.StockItems.AddRange(
            new ECommerce.Stock.Domain.StockItem { ProductId = 1, AvailableQuantity = 20 },
            new ECommerce.Stock.Domain.StockItem { ProductId = 2, AvailableQuantity = 15 },
            new ECommerce.Stock.Domain.StockItem { ProductId = 3, AvailableQuantity = 10 },
            new ECommerce.Stock.Domain.StockItem { ProductId = 4, AvailableQuantity = 8 },
            new ECommerce.Stock.Domain.StockItem { ProductId = 5, AvailableQuantity = 5 });
        db.SaveChanges();
    }
}

Log.Information("Stock worker ready");

host.Run();
