using ECommerce.Contracts.Common;
using MediatR;

namespace ECommerce.Stock.Application.Stock.Queries.GetStock;

public record GetStockQuery(int VariantId) : IRequest<Result<GetStockResult>>;

public record GetStockResult(
    int VariantId,
    int ProductId,
    int AvailableQuantity,
    int ReservedQuantity,
    int ActualAvailable
);
