using ECommerce.Product.Application.Common.Dtos;
using MediatR;

namespace ECommerce.Product.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(int ProductId) : IRequest<ProductDto?>;
