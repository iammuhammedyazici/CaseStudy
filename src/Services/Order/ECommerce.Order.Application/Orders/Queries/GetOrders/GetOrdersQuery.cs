using ECommerce.Contracts.Common;
using ECommerce.Order.Application.Orders.Dtos;
using MediatR;

namespace ECommerce.Order.Application.Orders.Queries.GetOrders;

public record GetOrdersQuery(
    string? UserId,
    string? Status,
    DateTime? FromDate,
    DateTime? ToDate,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<PagedResponse<OrderResponse>>>;
