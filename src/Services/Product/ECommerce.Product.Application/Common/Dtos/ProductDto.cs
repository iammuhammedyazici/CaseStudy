namespace ECommerce.Product.Application.Common.Dtos;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    string Category,
    string Brand,
    string ImageUrl,
    List<ProductVariantDto> Variants
);
