using ECommerce.Order.Domain;
using FluentValidation;

namespace ECommerce.Order.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Order must contain at least one item");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("Product ID must be greater than 0");

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0");

            item.RuleFor(x => x.UnitPrice)
                .GreaterThan(0)
                .WithMessage("Unit price must be greater than 0");
        });

        When(x => x.Source >= OrderSource.AmazonMarketplace && x.Source <= OrderSource.N11Marketplace, () =>
        {
            RuleFor(x => x.ExternalOrderId)
                .NotEmpty()
                .WithMessage("External Order ID is required for marketplace orders")
                .MaximumLength(100)
                .WithMessage("External Order ID cannot exceed 100 characters");

            RuleFor(x => x.ExternalSystemCode)
                .NotEmpty()
                .WithMessage("External System Code is required for marketplace orders")
                .MaximumLength(50)
                .WithMessage("External System Code cannot exceed 50 characters");
        });

        RuleFor(x => x.ShippingAddressId)
            .NotEmpty()
            .WithMessage("Shipping address is required");

        RuleFor(x => x.CustomerNote)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.CustomerNote))
            .WithMessage("Customer note cannot exceed 1000 characters");
    }
}
