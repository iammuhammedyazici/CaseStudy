using ECommerce.Order.Domain;

namespace ECommerce.Order.Application.Orders.Commands.CreateOrder;

public record CreateOrderItem(int ProductId, int VariantId, int Quantity, decimal UnitPrice);

public record CreateOrderResult(Guid OrderId, OrderStatus Status);
