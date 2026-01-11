using ECommerce.Notification.Application.Abstractions.Persistence;
using ECommerce.Notification.Domain;
using ECommerce.Notification.Infrastructure.Data;

namespace ECommerce.Notification.Infrastructure.Services;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _dbContext;

    public NotificationRepository(NotificationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void AddNotification(NotificationLog log)
    {
        _dbContext.NotificationLogs.Add(log);
    }
}
