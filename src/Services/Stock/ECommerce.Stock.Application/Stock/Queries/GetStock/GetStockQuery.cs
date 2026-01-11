using ECommerce.Contracts.Common;
using MediatR;

namespace ECommerce.Stock.Application.Stock.Queries.GetStock;

public record GetStockQuery(int VariantId) : IRequest<Result<GetStockResult>>;

public record GetStockResult(
    Guid VariantId,
    Guid ProductId,
    int AvailableQuantity,
    int ReservedQuantity,
    int ActualAvailable
);
