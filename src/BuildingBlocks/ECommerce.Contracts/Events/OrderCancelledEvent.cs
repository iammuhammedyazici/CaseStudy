namespace ECommerce.Contracts;

public record OrderCancelledEvent(
    Guid OrderId,
    string UserId,
    string Reason,
    DateTime CancelledAt,
    string? CancelledBy,
    List<OrderCancelledItem> Items
);

public record OrderCancelledItem(
    int ProductId,
    int Quantity
);
