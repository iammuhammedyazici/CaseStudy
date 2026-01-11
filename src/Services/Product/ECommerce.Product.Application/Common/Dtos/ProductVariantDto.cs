namespace ECommerce.Product.Application.Common.Dtos;

public record ProductVariantDto(
    int Id,
    string SKU,
    string Name,
    decimal Price,
    int StockQuantity,
    string? Color,
    string? Size
);
