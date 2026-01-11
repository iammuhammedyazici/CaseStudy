namespace ECommerce.Order.Domain;

public class OrderItem
{
    public Guid OrderId { get; set; }
    public int ProductId { get; set; }
    public int VariantId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public Order Order { get; set; } = null!;
}
