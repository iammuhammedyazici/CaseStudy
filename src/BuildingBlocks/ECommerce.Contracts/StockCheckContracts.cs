namespace ECommerce.Contracts;

public record CheckStockRequest(
    List<StockCheckItem> Items
);

public record StockCheckItem(
    int VariantId,
    int Quantity
);

public record CheckStockResponse(
    bool IsAvailable,
    List<UnavailableStockItem> UnavailableItems
);

public record UnavailableStockItem(
    int VariantId,
    int RequestedQuantity,
    int AvailableQuantity
);
