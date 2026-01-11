using ECommerce.Order.Infrastructure.Entities;
using ECommerce.Order.Infrastructure.Saga;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderEntity = ECommerce.Order.Domain.Order;
using OrderItemEntity = ECommerce.Order.Domain.OrderItem;

namespace ECommerce.Order.Infrastructure.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<OrderEntity> Orders => Set<OrderEntity>();
    public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();
    public DbSet<OrderState> OrderStates => Set<OrderState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderEntity>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Status).HasConversion<string>().HasMaxLength(50);
            entity.Property(o => o.Source).HasConversion<int>();
            entity.Property(o => o.ExternalOrderId).HasMaxLength(100);
            entity.Property(o => o.ExternalSystemCode).HasMaxLength(50);
            entity.Property(o => o.CancellationReason).HasMaxLength(500);
            entity.Property(o => o.CustomerNote).HasMaxLength(1000);
            entity.Property(o => o.InternalNote).HasMaxLength(1000);
            entity.Property(o => o.UpdatedBy).HasMaxLength(100);
            entity.Property(o => o.CancelledBy).HasMaxLength(100);

            entity.HasIndex(o => o.Source);
            entity.HasIndex(o => o.ExternalOrderId).IsUnique(false);
            entity.HasIndex(o => o.ExternalSystemCode);
            entity.HasIndex(o => o.ShippingAddressId);
            entity.HasIndex(o => o.Status);
            entity.HasIndex(o => o.CreatedAt);
        });

        modelBuilder.Entity<OrderItemEntity>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.VariantId });

            entity.Property(e => e.ProductId).IsRequired();
            entity.Property(e => e.VariantId).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();

            entity.HasIndex(e => e.VariantId);
            entity.HasIndex(e => e.ProductId);

            entity.HasOne(e => e.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Type).HasMaxLength(200);
            entity.Property(o => o.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<InboxMessage>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.HasIndex(i => i.MessageId).IsUnique();
            entity.Property(i => i.Consumer).HasMaxLength(200);
            entity.Property(i => i.Status).HasMaxLength(50);
        });

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
    }
}
