namespace ECommerce.Stock.Domain;

public class StockItem
{
    public int VariantId { get; set; }
    public int ProductId { get; set; }
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public uint RowVersion { get; set; }

    public int MinimumQuantity { get; set; }
    public bool IsActive { get; set; }
    public int ActualAvailable => AvailableQuantity - ReservedQuantity;
}
