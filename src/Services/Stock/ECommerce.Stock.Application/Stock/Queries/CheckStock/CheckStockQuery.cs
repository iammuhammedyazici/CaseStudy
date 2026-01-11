using ECommerce.Contracts.Common;
using MediatR;

namespace ECommerce.Stock.Application.Stock.Queries.CheckStock;

public record CheckStockQuery(List<StockCheckItem> Items) : IRequest<Result<List<CheckStockResult>>>;

public record StockCheckItem(int ProductId, int Quantity);

public record CheckStockResult(int ProductId, int Available, bool IsAvailable, int Requested);
