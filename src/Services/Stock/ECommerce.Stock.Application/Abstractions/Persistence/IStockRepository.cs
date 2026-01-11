using ECommerce.Stock.Domain;

namespace ECommerce.Stock.Application.Abstractions.Persistence;

public interface IStockRepository
{
    Task<StockItem?> GetStockItemAsync(int variantId, CancellationToken cancellationToken);
    Task<List<StockItem>> GetStockItemsAsync(IReadOnlyList<int> variantIds, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken ct);
    void AddReservation(StockReservation reservation);
}
