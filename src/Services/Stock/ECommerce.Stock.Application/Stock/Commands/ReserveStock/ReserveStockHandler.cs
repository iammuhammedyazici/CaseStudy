using ECommerce.Contracts;
using ECommerce.Stock.Application.Abstractions.Persistence;
using ECommerce.Stock.Domain;
using MediatR;

namespace ECommerce.Stock.Application.Stock.Commands.ReserveStock;

public class ReserveStockHandler : IRequestHandler<ReserveStockCommand, ReserveStockResult>
{
    private readonly IStockRepository _repository;

    public ReserveStockHandler(IStockRepository repository)
    {
        _repository = repository;
    }

    public async Task<ReserveStockResult> Handle(ReserveStockCommand request, CancellationToken ct)
    {
        var order = request.Order;
        var variantIds = order.Items.Select(i => i.VariantId).ToList();
        var stockItems = await _repository.GetStockItemsAsync(variantIds, ct);

        var failedItems = new List<StockReservationItem>();

        foreach (var item in order.Items)
        {
            var stockItem = stockItems.FirstOrDefault(s => s.VariantId == item.VariantId);

            if (stockItem == null)
            {
                failedItems.Add(new StockReservationItem(
                    item.ProductId,
                    item.VariantId,
                    item.Quantity,
                    0
                ));
                continue;
            }

            if (stockItem.ActualAvailable < item.Quantity)
            {
                failedItems.Add(new StockReservationItem(
                    stockItem.ProductId,
                    stockItem.VariantId,
                    item.Quantity,
                    stockItem.ActualAvailable
                ));
            }
        }

        if (failedItems.Any())
        {
            return new ReserveStockResult(false, "Failed to reserve stock for some items", failedItems);
        }

        foreach (var item in order.Items)
        {
            var stockItem = stockItems.First(s => s.VariantId == item.VariantId);
            stockItem.ReservedQuantity += item.Quantity;

            _repository.AddReservation(new StockReservation
            {
                Id = Guid.NewGuid(),
                OrderId = order.OrderId,
                ProductId = item.ProductId,
                VariantId = item.VariantId,
                Quantity = item.Quantity,
                ReservedAtUtc = DateTime.UtcNow
            });
        }

        await _repository.SaveChangesAsync(ct);
        return new ReserveStockResult(true, null, new List<StockReservationItem>());
    }
}
