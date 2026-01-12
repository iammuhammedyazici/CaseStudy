using ECommerce.Notification.Infrastructure.Data;
using ECommerce.Notification.Infrastructure;
using ECommerce.Notification.Worker.Extensions;
using ECommerce.Observability;
using ECommerce.Notification.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using MediatR;

var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();

LoggingExtensions.ConfigureSerilog("NotificationService", builder.Environment);
builder.Services.AddSerilog();

builder.Services.AddCustomMassTransit(builder.Configuration);
builder.Services.AddCustomOpenTelemetry(builder.Configuration);
builder.Services.AddCustomDbContext(builder.Configuration);
builder.Services.AddNotificationInfrastructure(builder.Configuration);
builder.Services.AddScoped<ECommerce.Notification.Application.Abstractions.Persistence.INotificationRepository, ECommerce.Notification.Infrastructure.Services.NotificationRepository>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(ECommerce.Notification.Application.DependencyInjection).Assembly
));
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
