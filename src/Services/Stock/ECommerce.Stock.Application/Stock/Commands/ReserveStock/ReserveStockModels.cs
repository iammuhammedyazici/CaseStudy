using ECommerce.Contracts;

namespace ECommerce.Stock.Application.Stock.Commands.ReserveStock;

public record ReserveStockResult(bool Success, string? Message, List<ECommerce.Contracts.StockReservationItem> FailedItems);
