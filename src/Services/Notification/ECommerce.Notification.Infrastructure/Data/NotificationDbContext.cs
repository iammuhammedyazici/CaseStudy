using ECommerce.Notification.Domain;
using ECommerce.Notification.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Notification.Infrastructure.Data;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();
    public DbSet<InboxMessage> InboxMessages => Set<InboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationLog>(entity =>
        {
            entity.HasKey(n => n.Id);
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
