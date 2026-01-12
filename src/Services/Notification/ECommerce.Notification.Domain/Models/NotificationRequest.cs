using ECommerce.Notification.Domain.Enums;

namespace ECommerce.Notification.Domain.Models;

/// <summary>
/// Represents a notification request with all necessary information
/// </summary>
public class NotificationRequest
{
    /// <summary>
    /// The channel through which the notification should be sent
    /// </summary>
    public required NotificationChannel Channel { get; init; }

    /// <summary>
    /// The recipient of the notification (email address or phone number)
    /// </summary>
    public required string Recipient { get; init; }

    /// <summary>
    /// The subject of the notification (for email)
    /// </summary>
    public string? Subject { get; init; }

    /// <summary>
    /// The content/body of the notification
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Related order ID (for logging purposes)
    /// </summary>
    public Guid? OrderId { get; init; }

    /// <summary>
    /// Additional metadata for the notification
    /// </summary>
    public Dictionary<string, string>? Metadata { get; init; }
}
