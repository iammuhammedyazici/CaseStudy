using ECommerce.Product.Application.Abstractions.Persistence;
using ECommerce.Product.Application.Common.Dtos;
using MediatR;

namespace ECommerce.Product.Application.Products.Queries.GetProductById;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _repository;

    public GetProductByIdHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
            return null;

        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Category,
            product.Brand,
            product.ImageUrl,
            product.Variants.Select(v => new ProductVariantDto(
                v.Id,
                v.SKU,
                v.Name,
                v.Price,
                v.StockQuantity,
                v.Color,
                v.Size
            )).ToList()
        );
    }
}
