using ECommerce.Product.Application.Abstractions.Search;
using ECommerce.Product.Application.Common.Dtos;
using Mapster;
using MediatR;

namespace ECommerce.Product.Application.Products.Queries.SearchProducts;

public class SearchProductsHandler : IRequestHandler<SearchProductsQuery, List<ProductDto>>
{
    private readonly IElasticsearchService _searchService;

    public SearchProductsHandler(IElasticsearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task<List<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _searchService.SearchProductsAsync(request.Query, cancellationToken);

        return products.Select(p => p.Adapt<ProductDto>()).ToList();
    }
}
