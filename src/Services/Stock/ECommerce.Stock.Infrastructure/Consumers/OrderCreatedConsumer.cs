using ECommerce.Contracts;
using ECommerce.Stock.Application.Stock.Commands.ReserveStock;
using ECommerce.Stock.Infrastructure.Data;
using ECommerce.Stock.Infrastructure.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Serilog;
using SerilogContext = Serilog.Context;
using ILogger = Serilog.ILogger;

namespace ECommerce.Stock.Infrastructure.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger _logger;

    public OrderCreatedConsumer(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _logger = Log.ForContext<OrderCreatedConsumer>();
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
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
                _logger.Error(ex, "Failed to process order.created message");
                throw;
            }
        }
    }

    private async Task ProcessMessageAsync(ConsumeContext<OrderCreatedEvent> context)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StockDbContext>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var inbox = new InboxMessage
            {
                Id = Guid.NewGuid(),
                MessageId = context.MessageId ?? Guid.NewGuid(),
                Consumer = nameof(OrderCreatedConsumer),
                ReceivedAtUtc = DateTime.UtcNow,
                Status = InboxStatus.Received
            };

            db.InboxMessages.Add(inbox);

            var result = await mediator.Send(new ReserveStockCommand(context.Message), CancellationToken.None);

            inbox.Status = InboxStatus.Processed;
            inbox.ProcessedAtUtc = DateTime.UtcNow;

            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            if (!result.Success)
            {
                await context.Publish(new StockReservationFailedEvent(
                    context.Message.OrderId,
                    result.Message ?? "Stock reservation failed",
                    result.FailedItems.ToList()));

                _logger.Information("Published StockReservationFailedEvent for Order {OrderId}", context.Message.OrderId);
            }
            else
            {
                await context.Publish(new StockReservedEvent(
                    context.Message.OrderId,
                    new List<StockReservationItem>()));

                _logger.Information("Published StockReservedEvent for Order {OrderId}", context.Message.OrderId);
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
