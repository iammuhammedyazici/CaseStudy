using ECommerce.Contracts;
using ECommerce.Stock.Application.Abstractions.Persistence;
using ECommerce.Stock.Application.Stock.Commands.ReserveStock;
using ECommerce.Stock.Domain;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerce.Stock.UnitTests.Stock.Commands.ReserveStock;

public class ReserveStockHandlerTests
{
    private readonly Mock<IStockRepository> _repositoryMock;
    private readonly ReserveStockHandler _handler;

    public ReserveStockHandlerTests()
    {
        _repositoryMock = new Mock<IStockRepository>();
        _handler = new ReserveStockHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReserveStock_When_StockIsAvailable()
    {
        var orderId = Guid.NewGuid();
        var order = new OrderCreatedEvent(
            orderId,
            "user-1",
            new List<OrderItem>
            {
                new(1, 101, 2, 50),
                new(2, 102, 1, 100)
            }
        );

        var command = new ReserveStockCommand(order);

        var stockItems = new List<StockItem>
        {
            new() { ProductId = 1, VariantId = 101, AvailableQuantity = 10, ReservedQuantity = 0 },
            new() { ProductId = 2, VariantId = 102, AvailableQuantity = 5, ReservedQuantity = 0 }
        };

        _repositoryMock
            .Setup(r => r.GetStockItemsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(stockItems);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.FailedItems.Should().BeEmpty();

        _repositoryMock.Verify(r => r.AddReservation(It.Is<StockReservation>(sr =>
            sr.OrderId == orderId && sr.VariantId == 101 && sr.Quantity == 2)), Times.Once);

        _repositoryMock.Verify(r => r.AddReservation(It.Is<StockReservation>(sr =>
            sr.OrderId == orderId && sr.VariantId == 102 && sr.Quantity == 1)), Times.Once);

        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_StockIsInsufficient()
    {
        var order = new OrderCreatedEvent(
            Guid.NewGuid(),
            "user-1",
            new List<OrderItem>
            {
                new(1, 101, 10, 50)
            }
        );

        var command = new ReserveStockCommand(order);

        var stockItems = new List<StockItem>
        {
            new() { ProductId = 1, VariantId = 101, AvailableQuantity = 5, ReservedQuantity = 0 }
        };

        _repositoryMock
            .Setup(r => r.GetStockItemsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(stockItems);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.FailedItems.Should().ContainSingle();
        result.FailedItems.First().VariantId.Should().Be(101);
        result.FailedItems.First().AvailableQuantity.Should().Be(5);

        _repositoryMock.Verify(r => r.AddReservation(It.IsAny<StockReservation>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Fail_When_StockItemNotFound()
    {
        var order = new OrderCreatedEvent(
            Guid.NewGuid(),
            "user-1",
            new List<OrderItem>
            {
                new(1, 999, 1, 50)
            }
        );

        var command = new ReserveStockCommand(order);

        _repositoryMock
            .Setup(r => r.GetStockItemsAsync(It.IsAny<List<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<StockItem>());

        var result = await _handler.Handle(command, CancellationToken.None);
        result.Success.Should().BeFalse();
        result.FailedItems.Should().ContainSingle();
        result.FailedItems.First().VariantId.Should().Be(999);
        result.FailedItems.First().AvailableQuantity.Should().Be(0);

        _repositoryMock.Verify(r => r.AddReservation(It.IsAny<StockReservation>()), Times.Never);
    }
}
