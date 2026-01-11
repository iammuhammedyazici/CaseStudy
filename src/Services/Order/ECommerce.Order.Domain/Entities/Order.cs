namespace ECommerce.Order.Domain;

public class Order
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.PendingStock;
    public DateTime CreatedAt { get; set; }
    public List<OrderItem> Items { get; set; } = new();

    public OrderSource Source { get; set; } = OrderSource.Web;

    public string? ExternalOrderId { get; set; }
    public string? ExternalSystemCode { get; set; }

    public Guid? ShippingAddressId { get; set; }
    public Guid? BillingAddressId { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public DateTime? CancelledAt { get; set; }
    public string? CancelledBy { get; set; }
    public string? CancellationReason { get; set; }

    public string? CustomerNote { get; set; }
    public string? InternalNote { get; set; }
}
