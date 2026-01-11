using ECommerce.Notification.Infrastructure.Data;
using ECommerce.Notification.Worker.Extensions;
using ECommerce.Observability;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();

LoggingExtensions.ConfigureSerilog("NotificationService", builder.Environment);
builder.Services.AddSerilog();

builder.Services.AddCustomMassTransit(builder.Configuration);
builder.Services.AddCustomOpenTelemetry(builder.Configuration);
builder.Logging.AddCustomOpenTelemetry(builder.Configuration);

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
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
}

Log.Information("Notification worker ready");

host.Run();
