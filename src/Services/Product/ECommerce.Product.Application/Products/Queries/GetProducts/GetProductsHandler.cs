using ECommerce.Product.Application.Abstractions.Persistence;
using ECommerce.Product.Application.Common.Dtos;
using MediatR;

namespace ECommerce.Product.Application.Products.Queries.GetProducts;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _repository;

    public GetProductsHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.GetAllAsync(cancellationToken);

        return products.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Description,
            p.Category,
            p.Brand,
            p.ImageUrl,
            p.Variants.Select(v => new ProductVariantDto(
                v.Id,
                v.SKU,
                v.Name,
                v.Price,
                v.StockQuantity,
                v.Color,
                v.Size
            )).ToList()
        )).ToList();
    }
}
