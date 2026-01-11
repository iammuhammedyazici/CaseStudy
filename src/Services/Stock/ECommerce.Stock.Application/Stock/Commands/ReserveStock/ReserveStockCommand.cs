using ECommerce.Contracts;
using MediatR;

namespace ECommerce.Stock.Application.Stock.Commands.ReserveStock;

public record ReserveStockCommand(OrderCreatedEvent Order) : IRequest<ReserveStockResult>;
