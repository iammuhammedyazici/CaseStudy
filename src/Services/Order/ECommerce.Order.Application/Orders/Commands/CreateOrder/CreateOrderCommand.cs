using ECommerce.Contracts.Common;
using ECommerce.Order.Domain;
using MediatR;

namespace ECommerce.Order.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(
    IReadOnlyList<CreateOrderItem> Items,
    OrderSource Source = OrderSource.Web,
    string? UserId = null,
    string? ExternalOrderId = null,
    string? ExternalSystemCode = null,
    Guid? ShippingAddressId = null,
    Guid? BillingAddressId = null,
    string? CustomerNote = null,
    string? IdempotencyKey = null,
    string? GuestEmail = null
) : IRequest<Result<CreateOrderResult>>;
