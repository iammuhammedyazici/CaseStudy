using ECommerce.Contracts;
using ECommerce.Order.Application.Abstractions;
using ECommerce.Order.Application.Abstractions.Persistence;
using ECommerce.Order.Application.Orders.Commands.CreateOrder;
using ECommerce.Order.Application.Orders.Dtos;
using ECommerce.Order.Domain;
using FluentAssertions;
using FluentValidation;
using FluentValidationResult = FluentValidation.Results.ValidationResult;
using FluentValidation.Results;
using MassTransit;
using Moq;
using Xunit;

namespace ECommerce.Order.UnitTests.Orders.Commands.CreateOrder;

public class CreateOrderHandlerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock;
    private readonly Mock<IValidator<CreateOrderCommand>> _validatorMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly CreateOrderHandler _handler;

    private readonly Mock<IPublishEndpoint> _publishEndpointMock;
    private readonly Mock<IRequestClient<CheckStockRequest>> _requestClientMock;

    public CreateOrderHandlerTests()
    {
        _repositoryMock = new Mock<IOrderRepository>();
        _validatorMock = new Mock<IValidator<CreateOrderCommand>>(MockBehavior.Strict);
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _requestClientMock = new Mock<IRequestClient<CheckStockRequest>>();
        _handler = new CreateOrderHandler(_repositoryMock.Object, _validatorMock.Object, _publishEndpointMock.Object, _currentUserServiceMock.Object, _requestClientMock.Object);
    }

    [Fact]
    public async Task Handle_Should_CreateOrder_When_RequestIsValid()
    {
        _currentUserServiceMock.Setup(x => x.UserId).Returns("user-1");

        var command = new CreateOrderCommand(
            Items: new List<CreateOrderItem>
            {
                new(1, 1, 2, 100)
            },
            UserId: "user-1",
            ShippingAddress: new AddressDto("John Doe", "1234567890", "123 Main St", null, "City", "State", "12345", "Country"),
            BillingAddress: new AddressDto("John Doe", "1234567890", "123 Main St", null, "City", "State", "12345", "Country"),
            Source: OrderSource.Web,
            ExternalOrderId: "ext-1",
            ExternalSystemCode: "sys-1",
            CustomerNote: "note"
        );

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidationResult());

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateOrderCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidationResult());

        var stockResponseMock = new Mock<Response<CheckStockResponse>>();
        stockResponseMock.Setup(r => r.Message).Returns(new CheckStockResponse(true, new List<UnavailableStockItem>()));

        _requestClientMock
            .Setup(c => c.GetResponse<CheckStockResponse>(It.IsAny<CheckStockRequest>(), It.IsAny<CancellationToken>(), It.IsAny<RequestTimeout>()))
            .ReturnsAsync(stockResponseMock.Object);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Status.Should().Be(OrderStatus.PendingStock);

        _repositoryMock.Verify(r => r.AddAsync(
            It.Is<ECommerce.Order.Domain.Order>(o =>
                o.UserId == command.UserId &&
                o.TotalAmount == 200 &&
                o.Items.Count == 1 &&
                o.Items.First().ProductId == 1
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        _publishEndpointMock.Verify(p => p.Publish(
            It.Is<OrderCreatedEvent>(e =>
                e.UserId == command.UserId &&
                e.Items.Count == 1
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ThrowValidationException_When_RequestIsInvalid()
    {
        _currentUserServiceMock.Setup(x => x.UserId).Returns("user-1");

        var command = new CreateOrderCommand(
            Items: new List<CreateOrderItem>(),
            ShippingAddress: null,
            BillingAddress: null,
            Source: OrderSource.Web,
            ExternalOrderId: null,
            ExternalSystemCode: null,
            CustomerNote: null
        );

        var validationFailure = new ValidationFailure("UserId", "UserId is required");
        var validationResult = new FluentValidationResult(new[] { validationFailure });

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException(new[] { validationFailure }));

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateOrderCommand>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ValidationException(new[] { validationFailure }));

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();

        _repositoryMock.Verify(r => r.AddAsync(
            It.IsAny<ECommerce.Order.Domain.Order>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);

        _publishEndpointMock.Verify(p => p.Publish(
            It.IsAny<OrderCreatedEvent>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }
}
