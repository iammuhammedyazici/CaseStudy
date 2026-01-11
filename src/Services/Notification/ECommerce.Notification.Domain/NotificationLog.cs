namespace ECommerce.Notification.Domain;

public class NotificationLog
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}
