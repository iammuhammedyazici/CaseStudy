using ECommerce.Contracts.Common;
using MediatR;

namespace ECommerce.Order.Application.Orders.Commands.CancelOrder;

public record CancelOrderCommand(Guid OrderId, string Reason) : IRequest<Result>;
