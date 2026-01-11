using ECommerce.Product.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Product.Infrastructure.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.Product> Products => Set<Domain.Entities.Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Entities.Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Description).IsRequired().HasMaxLength(2000);
            entity.Property(p => p.Category).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Brand).IsRequired().HasMaxLength(100);
            entity.Property(p => p.ImageUrl).HasMaxLength(500);
            entity.Property(p => p.IsActive).IsRequired();
            entity.Property(p => p.CreatedAt).IsRequired();

            entity.HasMany(p => p.Variants)
                  .WithOne(v => v.Product)
                  .HasForeignKey(v => v.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.SKU).IsRequired().HasMaxLength(50);
            entity.Property(v => v.Name).IsRequired().HasMaxLength(200);
            entity.Property(v => v.Price).IsRequired().HasPrecision(18, 2);
            entity.Property(v => v.StockQuantity).IsRequired();
            entity.Property(v => v.Color).HasMaxLength(50);
            entity.Property(v => v.Size).HasMaxLength(20);
            entity.Property(v => v.Material).HasMaxLength(100);
            entity.Property(v => v.ImageUrl).HasMaxLength(500);
            entity.Property(v => v.IsActive).IsRequired();
            entity.Property(v => v.CreatedAt).IsRequired();

            entity.HasIndex(v => v.SKU).IsUnique();
            entity.HasIndex(v => v.ProductId);
        });
    }
}
