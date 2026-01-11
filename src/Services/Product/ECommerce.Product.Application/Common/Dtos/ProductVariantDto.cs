namespace ECommerce.Product.Application.Common.Dtos;

public record ProductVariantDto(
    Guid Id,
    string SKU,
    string Name,
    decimal Price,
    int StockQuantity,
    string? Color,
    string? Size
);
