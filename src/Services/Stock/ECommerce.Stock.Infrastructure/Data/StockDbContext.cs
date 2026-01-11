using ECommerce.Stock.Domain;
using ECommerce.Stock.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Stock.Infrastructure.Data;

public class StockDbContext : DbContext
{
    public StockDbContext(DbContextOptions<StockDbContext> options) : base(options)
    {
    }

    public DbSet<StockItem> StockItems => Set<StockItem>();
    public DbSet<StockReservation> StockReservations => Set<StockReservation>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockItem>(entity =>
        {
            entity.HasKey(e => e.VariantId);
            entity.Property(e => e.ProductId).IsRequired();
            entity.Property(e => e.AvailableQuantity).IsRequired();
            entity.Property(e => e.ReservedQuantity).IsRequired().HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.MinimumQuantity).HasDefaultValue(0);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.RowVersion).IsRowVersion();

            entity.HasIndex(e => e.ProductId);
        });

        modelBuilder.Entity<StockReservation>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.OrderId).IsRequired();
            entity.Property(s => s.ProductId).IsRequired();
            entity.Property(s => s.VariantId).IsRequired();
            entity.Property(s => s.Quantity).IsRequired();
            entity.Property(s => s.ReservedAtUtc).IsRequired();

            entity.HasIndex(s => s.OrderId);
            entity.HasIndex(s => s.VariantId);
        });

        modelBuilder.Entity<InboxMessage>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.HasIndex(i => i.MessageId).IsUnique();
            entity.Property(i => i.Consumer).HasMaxLength(200);
            entity.Property(i => i.Status).HasMaxLength(50);
        });
    }
}
