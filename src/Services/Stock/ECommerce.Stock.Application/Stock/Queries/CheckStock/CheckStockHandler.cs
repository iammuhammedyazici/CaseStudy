using ECommerce.Stock.Application.Abstractions.Persistence;
using ECommerce.Contracts.Common;
using MediatR;

namespace ECommerce.Stock.Application.Stock.Queries.CheckStock;

public class CheckStockHandler : IRequestHandler<CheckStockQuery, Result<List<CheckStockResult>>>
{
    private readonly IStockRepository _repository;

    public CheckStockHandler(IStockRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<CheckStockResult>>> Handle(CheckStockQuery request, CancellationToken ct)
    {
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var stockItems = await _repository.GetStockItemsAsync(productIds, ct);

        var results = new List<CheckStockResult>();
        foreach (var item in request.Items)
        {
            var stock = stockItems.FirstOrDefault(s => s.ProductId == item.ProductId);
            var available = stock?.ActualAvailable ?? 0;
            var isAvailable = available >= item.Quantity;

            results.Add(new CheckStockResult(
                item.ProductId,
                available,
                isAvailable,
                item.Quantity
            ));
        }

        return Result<List<CheckStockResult>>.Success(results);
    }
}
