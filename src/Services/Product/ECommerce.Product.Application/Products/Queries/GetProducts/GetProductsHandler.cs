using ECommerce.Product.Application.Abstractions.Persistence;
using ECommerce.Product.Application.Common.Dtos;
using Mapster;
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

        return products.Select(p => p.Adapt<ProductDto>()).ToList();
    }
}
