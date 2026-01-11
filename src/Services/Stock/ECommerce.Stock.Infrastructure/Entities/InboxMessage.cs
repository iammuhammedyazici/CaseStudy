namespace ECommerce.Stock.Infrastructure.Entities;

public class InboxMessage
{
    public Guid Id { get; set; }
    public Guid MessageId { get; set; }
    public string Consumer { get; set; } = string.Empty;
    public DateTime ReceivedAtUtc { get; set; }
    public DateTime? ProcessedAtUtc { get; set; }
    public string Status { get; set; } = InboxStatus.Received;
}

public static class InboxStatus
{
    public const string Received = "Received";
    public const string Processed = "Processed";
}
