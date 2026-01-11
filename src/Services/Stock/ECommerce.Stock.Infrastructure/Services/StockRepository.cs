using ECommerce.Stock.Application.Abstractions.Persistence;
using ECommerce.Stock.Domain;
using ECommerce.Stock.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Stock.Infrastructure.Services;

public class StockRepository : IStockRepository
{
    private readonly StockDbContext _dbContext;

    public StockRepository(StockDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StockItem?> GetStockItemAsync(int variantId, CancellationToken cancellationToken)
    {
        return await _dbContext.StockItems
            .FirstOrDefaultAsync(s => s.VariantId == variantId, cancellationToken);
    }

    public async Task<List<StockItem>> GetStockItemsAsync(IReadOnlyList<int> variantIds, CancellationToken cancellationToken)
    {
        return await _dbContext.StockItems
            .Where(s => variantIds.Contains(s.VariantId))
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _dbContext.SaveChangesAsync(ct);
    }

    public void AddReservation(StockReservation reservation)
    {
        _dbContext.StockReservations.Add(reservation);
    }
}
