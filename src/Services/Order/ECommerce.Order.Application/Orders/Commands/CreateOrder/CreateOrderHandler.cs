using ECommerce.Contracts;
using ECommerce.Contracts.Common;
using ECommerce.Order.Application.Abstractions;
using ECommerce.Order.Application.Abstractions.Persistence;
using ECommerce.Order.Domain;
using OrderEntity = ECommerce.Order.Domain.Order;
using FluentValidation;
using MassTransit;
using MediatR;

namespace ECommerce.Order.Application.Orders.Commands.CreateOrder;

/// <summary>
/// Handler for creating a new order.
/// Validates the request, creates the order entity, saves it to the database, and publishes an OrderCreatedEvent.
/// </summary>
public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result<CreateOrderResult>>
{
    private readonly IOrderRepository _repository;
    private readonly IValidator<CreateOrderCommand> _validator;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ICurrentUserService _currentUserService;

    public CreateOrderHandler(IOrderRepository repository, IValidator<CreateOrderCommand> validator, IPublishEndpoint publishEndpoint, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _validator = validator;
        _publishEndpoint = publishEndpoint;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Handles the creation of a new order.
    /// </summary>
    /// <param name="request">The create order command containing order details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the order creation process.</returns>
    public async Task<Result<CreateOrderResult>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId ?? _currentUserService.UserId;

        if (string.IsNullOrEmpty(userId))
        {
            return Result<CreateOrderResult>.Unauthorized("User is not authenticated.");
        }

        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        var order = new OrderEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.PendingStock,
            TotalAmount = request.Items.Sum(i => i.UnitPrice * i.Quantity),
            Source = request.Source,
            ExternalOrderId = request.ExternalOrderId,
            ExternalSystemCode = request.ExternalSystemCode,
            ShippingAddressId = request.ShippingAddressId,
            BillingAddressId = request.BillingAddressId,
            CustomerNote = request.CustomerNote,
            Items = request.Items.Select(i => new ECommerce.Order.Domain.OrderItem
            {
                ProductId = i.ProductId,
                VariantId = i.VariantId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        await _repository.AddAsync(order, cancellationToken);

        await _publishEndpoint.Publish(new OrderCreatedEvent(
            order.Id,
            order.UserId,
            request.Items.Select(i => new ECommerce.Contracts.OrderItem(i.ProductId, i.VariantId, i.Quantity, i.UnitPrice)).ToList()), cancellationToken);

        return Result<CreateOrderResult>.Success(new CreateOrderResult(order.Id, order.Status));
    }
}
