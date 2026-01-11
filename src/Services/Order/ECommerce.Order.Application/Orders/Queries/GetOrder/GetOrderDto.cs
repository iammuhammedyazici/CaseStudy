using ECommerce.Order.Domain;

namespace ECommerce.Order.Application.Orders.Queries.GetOrder;

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public Guid VariantId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public record GetOrderDto(
    Guid Id,
    string UserId,
    decimal TotalAmount,
    OrderStatus Status,
    DateTime CreatedAt,
    IReadOnlyList<OrderItemDto> Items,
    OrderSource Source,
    string? ExternalOrderId,
    string? ExternalSystemCode,
    Guid? ShippingAddressId,
    Guid? BillingAddressId,
    DateTime? UpdatedAt,
    DateTime? CancelledAt,
    string? CancellationReason,
    string? CustomerNote
);
