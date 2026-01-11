using ECommerce.Product.Application.Abstractions.Persistence;
using ECommerce.Product.Domain.Entities;
using ECommerce.Product.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Product.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;

    public ProductRepository(ProductDbContext context)
    {
        _context = context;
    }

    public async Task<List<Domain.Entities.Product>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Products
            .Include(p => p.Variants.Where(v => v.IsActive))
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Products
            .Include(p => p.Variants.Where(v => v.IsActive))
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive, cancellationToken);
    }

    public async Task<List<ProductVariant>> GetVariantsByProductIdAsync(int productId, CancellationToken cancellationToken)
    {
        return await _context.ProductVariants
            .Where(v => v.ProductId == productId && v.IsActive)
            .OrderBy(v => v.Name)
            .ToListAsync(cancellationToken);
    }
}
