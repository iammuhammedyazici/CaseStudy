using System.Text.Json;
using ECommerce.Contracts;
using ECommerce.Order.Application.Abstractions.Persistence;
using ECommerce.Order.Infrastructure.Data;
using ECommerce.Order.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using OrderEntity = ECommerce.Order.Domain.Order;

namespace ECommerce.Order.Infrastructure.Services;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _dbContext;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public OrderRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(OrderEntity order, CancellationToken cancellationToken)
    {
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid> CreateOrderWithOutboxAsync(OrderEntity order, MessageEnvelope<OrderCreatedEvent> envelope, CancellationToken cancellationToken)
    {
        var outbox = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = envelope.EventType,
            Payload = JsonSerializer.Serialize(envelope, _jsonOptions),
            OccurredAtUtc = envelope.OccurredAtUtc,
            Status = OutboxStatus.Pending
        };

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        _dbContext.Orders.Add(order);
        _dbContext.OutboxMessages.Add(outbox);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return order.Id;
    }

    public async Task<OrderEntity?> GetOrderAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<OrderEntity> Orders, int TotalCount)> GetPagedOrdersAsync(
        string? userId,
        string? status,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Orders
            .Include(o => o.Items)
            .AsQueryable();

        if (!string.IsNullOrEmpty(userId))
        {
            query = query.Where(o => o.UserId == userId);
        }

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<Domain.OrderStatus>(status, out var orderStatus))
        {
            query = query.Where(o => o.Status == orderStatus);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= toDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (orders, totalCount);
    }

    public async Task UpdateOrderAsync(OrderEntity order, CancellationToken cancellationToken)
    {
        _dbContext.Orders.Update(order);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task PublishEventAsync<T>(MessageEnvelope<T> envelope, CancellationToken cancellationToken)
    {
        var outbox = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = envelope.EventType,
            Payload = JsonSerializer.Serialize(envelope, _jsonOptions),
            OccurredAtUtc = envelope.OccurredAtUtc,
            Status = OutboxStatus.Pending
        };

        _dbContext.OutboxMessages.Add(outbox);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
