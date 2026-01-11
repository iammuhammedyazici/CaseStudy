using ECommerce.Contracts;
using ECommerce.Contracts.Common;
using ECommerce.Order.Application.Abstractions.Persistence;
using ECommerce.Order.Domain;
using MediatR;

using ECommerce.Order.Application.Abstractions;

namespace ECommerce.Order.Application.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result>
{
    private readonly IOrderRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public CancelOrderCommandHandler(IOrderRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetOrderAsync(request.OrderId, cancellationToken);

        if (order == null)
        {
            return Result.NotFound("Order not found");
        }

        var userId = _currentUserService.UserId;
        var isAdmin = _currentUserService.IsAdmin;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.Unauthorized("User is not authenticated");
        }

        if (order.UserId != userId && !isAdmin)
        {
            return Result.Failure("Forbidden - not your order");
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            return Result.Conflict("Order is already cancelled");
        }

        if (order.Status == OrderStatus.Confirmed)
        {
            return Result.Unprocessable("Cannot cancel confirmed order. Please contact support.");
        }

        order.Status = OrderStatus.Cancelled;
        order.CancelledAt = DateTime.UtcNow;
        order.CancelledBy = userId;
        order.CancellationReason = request.Reason;
        order.UpdatedAt = DateTime.UtcNow;
        order.UpdatedBy = userId;

        await _repository.UpdateOrderAsync(order, cancellationToken);

        var cancelledEvent = new OrderCancelledEvent(
            order.Id,
            order.UserId,
            request.Reason,
            order.CancelledAt.Value,
            order.CancelledBy,
            order.Items.Select(i => new OrderCancelledItem(i.ProductId, i.Quantity)).ToList()
        );

        var envelope = new MessageEnvelope<OrderCancelledEvent>(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            "OrderCancelledEvent",
            cancelledEvent
        );

        await _repository.PublishEventAsync(envelope, cancellationToken);

        return Result.Success();
    }
}
