namespace ECommerce.Stock.Domain;

public class StockReservation
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public int ProductId { get; set; }
    public int VariantId { get; set; }
    public int Quantity { get; set; }
    public DateTime ReservedAtUtc { get; set; }
}
