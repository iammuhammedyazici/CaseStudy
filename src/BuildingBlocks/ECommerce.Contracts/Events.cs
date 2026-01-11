namespace ECommerce.Contracts;

public static class EventTypes
{
    public const string OrderCreated = "order.created";
    public const string StockReserved = "stock.reserved";
    public const string StockFailed = "stock.failed";
}

public record MessageEnvelope<TPayload>(
    Guid MessageId,
    Guid CorrelationId,
    DateTime OccurredAtUtc,
    string EventType,
    TPayload Payload);

public record OrderItem(int ProductId, int VariantId, int Quantity, decimal UnitPrice);

public record OrderCreatedEvent(
    Guid OrderId,
    string UserId,
    IReadOnlyList<OrderItem> Items);

public record StockReservationItem(int ProductId, int VariantId, int Quantity, int AvailableQuantity);

public record StockReservedEvent(
    Guid OrderId,
    IReadOnlyList<StockReservationItem> ReservedItems);

public record StockReservationFailedEvent(
    Guid OrderId,
    string Reason,
    IReadOnlyList<StockReservationItem> FailedItems);
