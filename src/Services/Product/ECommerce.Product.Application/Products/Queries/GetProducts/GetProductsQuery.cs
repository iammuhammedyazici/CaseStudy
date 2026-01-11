using ECommerce.Product.Application.Common.Dtos;
using MediatR;

namespace ECommerce.Product.Application.Products.Queries.GetProducts;

public record GetProductsQuery() : IRequest<List<ProductDto>>;
