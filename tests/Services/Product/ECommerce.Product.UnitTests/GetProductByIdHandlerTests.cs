using ECommerce.Product.Application.Abstractions.Persistence;
using ECommerce.Product.Application.Products.Queries.GetProductById;
using ECommerce.Product.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace ECommerce.Product.UnitTests.Products.Queries.GetProductById;

public class GetProductByIdHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly GetProductByIdHandler _handler;

    public GetProductByIdHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _handler = new GetProductByIdHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnProduct_When_ProductExists()
    {
        var productId = 1;
        var product = new ECommerce.Product.Domain.Entities.Product
        {
            Id = productId,
            Name = "Test Product",
            Description = "Test Description",
            Category = "Electronics",
            Brand = "Test Brand",
            ImageUrl = "http://test.com/image.jpg",
            Variants = new List<ProductVariant>
            {
                new()
                {
                    Id = 101,
                    ProductId = productId,
                    SKU = "SKU-101",
                    Name = "Variant 1",
                    Price = 100,
                    StockQuantity = 10,
                    Color = "Red",
                    Size = "M"
                }
            }
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var query = new GetProductByIdQuery(productId);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.Name.Should().Be("Test Product");
        result.Variants.Should().HaveCount(1);
        result.Variants.First().Id.Should().Be(101);
        result.Variants.First().SKU.Should().Be("SKU-101");

        _repositoryMock.Verify(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnNull_When_ProductDoesNotExist()
    {
        var productId = 999;
        _repositoryMock
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ECommerce.Product.Domain.Entities.Product?)null);

        var query = new GetProductByIdQuery(productId);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();

        _repositoryMock.Verify(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
