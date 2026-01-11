using ECommerce.Contracts;
using ECommerce.Notification.Application.Notifications.Commands.SendNotification;
using ECommerce.Notification.Infrastructure.Data;
using ECommerce.Notification.Infrastructure.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;
using SerilogContext = Serilog.Context;
using ILogger = Serilog.ILogger;

namespace ECommerce.Notification.Infrastructure.Consumers;

public class StockFailedConsumer : IConsumer<StockReservationFailedEvent>
{
    private readonly NotificationDbContext _db;
    private readonly IMediator _mediator;
    private readonly ILogger _logger;

    public StockFailedConsumer(NotificationDbContext db, IMediator mediator)
    {
        _db = db;
        _mediator = mediator;
        _logger = Log.ForContext<StockFailedConsumer>();
    }

    public async Task Consume(ConsumeContext<StockReservationFailedEvent> context)
    {
        using (SerilogContext.LogContext.PushProperty("CorrelationId", context.CorrelationId))
        using (SerilogContext.LogContext.PushProperty("MessageId", context.MessageId))
        {
            try
            {
                await ProcessMessageAsync(context);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to process StockReservationFailedEvent");
                throw;
            }
        }
    }

    private async Task ProcessMessageAsync(ConsumeContext<StockReservationFailedEvent> context)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var inbox = new InboxMessage
            {
                Id = Guid.NewGuid(),
                MessageId = context.MessageId ?? Guid.NewGuid(),
                Consumer = nameof(StockFailedConsumer),
                ReceivedAtUtc = DateTime.UtcNow,
                Status = InboxStatus.Received
            };

            _db.InboxMessages.Add(inbox);

            await _mediator.Send(new SendNotificationCommand(context.Message.OrderId, "Failed"), CancellationToken.None);

            inbox.Status = InboxStatus.Processed;
            inbox.ProcessedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            using (SerilogContext.LogContext.PushProperty("OrderId", context.Message.OrderId))
            {
                _logger.Information("Notification logged for stock status Failed");
            }
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            await transaction.RollbackAsync();
            _logger.Information("Message already processed");
        }
    }

    private static bool IsUniqueViolation(DbUpdateException ex)
        => ex.InnerException is PostgresException pg && pg.SqlState == PostgresErrorCodes.UniqueViolation;
}
