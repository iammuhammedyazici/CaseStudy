using ECommerce.Product.Domain.Entities;

namespace ECommerce.Product.Application.Abstractions.Persistence;

public interface IProductRepository
{
    Task<List<Domain.Entities.Product>> GetAllAsync(CancellationToken cancellationToken);
    Task<Domain.Entities.Product?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<List<ProductVariant>> GetVariantsByProductIdAsync(int productId, CancellationToken cancellationToken);
}
