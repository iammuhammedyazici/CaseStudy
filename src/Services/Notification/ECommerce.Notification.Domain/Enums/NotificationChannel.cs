namespace ECommerce.Notification.Domain.Enums;

/// <summary>
/// Represents the available notification channels
/// </summary>
public enum NotificationChannel
{
    /// <summary>
    /// Email notification
    /// </summary>
    Email = 1,

    /// <summary>
    /// SMS notification
    /// </summary>
    Sms = 2,

    /// <summary>
    /// Push notification (future implementation)
    /// </summary>
    Push = 3
}
