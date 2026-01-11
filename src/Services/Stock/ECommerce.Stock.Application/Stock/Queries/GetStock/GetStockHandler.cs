using ECommerce.Stock.Application.Abstractions.Persistence;
using ECommerce.Contracts.Common;
using Mapster;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace ECommerce.Stock.Application.Stock.Queries.GetStock;

public class GetStockHandler : IRequestHandler<GetStockQuery, Result<GetStockResult>>
{
    private readonly IStockRepository _repository;
    private readonly Microsoft.Extensions.Caching.Distributed.IDistributedCache _cache;

    public GetStockHandler(IStockRepository repository, Microsoft.Extensions.Caching.Distributed.IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<Result<GetStockResult>> Handle(GetStockQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"stock:{request.VariantId}";
        var cachedStock = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (!string.IsNullOrWhiteSpace(cachedStock))
        {
            var cachedItem = System.Text.Json.JsonSerializer.Deserialize<GetStockResult>(cachedStock);
            if (cachedItem != null)
            {
                return Result<GetStockResult>.Success(cachedItem);
            }
        }

        var stockItem = await _repository.GetStockItemAsync(request.VariantId, cancellationToken);

        if (stockItem == null)
        {
            return Result<GetStockResult>.Failure("Product not found");
        }

        var result = stockItem.Adapt<GetStockResult>();

        var cacheOptions = new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };

        await _cache.SetStringAsync(
            cacheKey,
            System.Text.Json.JsonSerializer.Serialize(result),
            cacheOptions,
            cancellationToken);

        return Result<GetStockResult>.Success(result);
    }
}
