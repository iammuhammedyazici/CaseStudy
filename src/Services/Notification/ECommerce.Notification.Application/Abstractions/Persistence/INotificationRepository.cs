using ECommerce.Notification.Domain;

namespace ECommerce.Notification.Application.Abstractions.Persistence;

public interface INotificationRepository
{
    void AddNotification(NotificationLog log);
}
