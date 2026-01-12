using ECommerce.Order.Infrastructure.Entities;
using ECommerce.Order.Infrastructure.Saga;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ECommerce.Order.Domain;
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
    public DbSet<OutboxState> OutboxState => Set<OutboxState>();
    public DbSet<InboxState> InboxState => Set<InboxState>();
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
            entity.Property(o => o.IdempotencyKey).HasMaxLength(100);
            entity.Property(o => o.GuestEmail).HasMaxLength(255);

            entity.HasIndex(o => o.Source);
            entity.HasIndex(o => o.ExternalOrderId).IsUnique(false);
            entity.HasIndex(o => o.ExternalSystemCode);
            entity.HasIndex(o => o.IdempotencyKey).IsUnique();
            entity.HasIndex(o => o.GuestEmail);
            entity.HasIndex(o => o.Status);
            entity.HasIndex(o => o.CreatedAt);

            entity.HasOne(o => o.ShippingAddress)
                .WithOne()
                .HasForeignKey<OrderAddress>("ShippingAddressOrderId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(o => o.BillingAddress)
                .WithOne()
                .HasForeignKey<OrderAddress>("BillingAddressOrderId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderAddress>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.AddressType).HasMaxLength(20);
            entity.Property(a => a.FullName).HasMaxLength(100);
            entity.Property(a => a.Phone).HasMaxLength(20);
            entity.Property(a => a.AddressLine1).HasMaxLength(200);
            entity.Property(a => a.AddressLine2).HasMaxLength(200);
            entity.Property(a => a.City).HasMaxLength(100);
            entity.Property(a => a.State).HasMaxLength(100);
            entity.Property(a => a.PostalCode).HasMaxLength(20);
            entity.Property(a => a.Country).HasMaxLength(100);
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

        modelBuilder.Entity<OutboxState>(entity =>
        {
            entity.HasKey(x => x.OutboxId);
            entity.Property(x => x.RowVersion).IsRowVersion();
        });

        modelBuilder.Entity<InboxState>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.MessageId).IsUnique();
            entity.Property(x => x.RowVersion).IsRowVersion();
        });

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
    }
}
