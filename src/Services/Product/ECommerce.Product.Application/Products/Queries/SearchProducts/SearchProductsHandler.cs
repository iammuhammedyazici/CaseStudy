using ECommerce.Contracts.Common;
using ECommerce.Product.Application.Abstractions.Search;
using ECommerce.Product.Application.Common.Dtos;
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

        return products.Select(p => new ProductDto(
            GuidHelper.ToGuid(p.Id),
            p.Name,
            p.Description,
            p.Category,
            p.Brand,
            p.ImageUrl,
            p.Variants.Select(v => new ProductVariantDto(
                GuidHelper.ToGuid(v.Id),
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
