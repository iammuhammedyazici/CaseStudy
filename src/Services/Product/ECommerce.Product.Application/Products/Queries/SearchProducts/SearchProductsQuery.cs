using ECommerce.Product.Application.Common.Dtos;
using MediatR;

namespace ECommerce.Product.Application.Products.Queries.SearchProducts;

public record SearchProductsQuery(string Query) : IRequest<List<ProductDto>>;
