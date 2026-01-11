using ECommerce.Contracts;
using ECommerce.Order.Domain;
using OrderEntity = ECommerce.Order.Domain.Order;

namespace ECommerce.Order.Application.Abstractions.Persistence;

public interface IOrderRepository
{
    Task AddAsync(OrderEntity order, CancellationToken cancellationToken);
    Task<Guid> CreateOrderWithOutboxAsync(OrderEntity order, MessageEnvelope<OrderCreatedEvent> envelope, CancellationToken cancellationToken);
    Task<OrderEntity?> GetOrderAsync(Guid id, CancellationToken cancellationToken);
    Task<(IReadOnlyList<OrderEntity> Orders, int TotalCount)> GetPagedOrdersAsync(
        string? userId,
        string? status,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
    Task UpdateOrderAsync(OrderEntity order, CancellationToken cancellationToken);
    Task PublishEventAsync<T>(MessageEnvelope<T> envelope, CancellationToken cancellationToken);
}
