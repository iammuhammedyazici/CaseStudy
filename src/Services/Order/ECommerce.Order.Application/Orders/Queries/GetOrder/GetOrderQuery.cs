using ECommerce.Contracts.Common;
using ECommerce.Order.Application.Orders.Dtos;
using MediatR;

namespace ECommerce.Order.Application.Orders.Queries.GetOrder;

public record GetOrderQuery(Guid OrderId) : IRequest<Result<OrderResponse>>;
