namespace ECommerce.Order.Infrastructure.Entities;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime OccurredAtUtc { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public string Status { get; set; } = OutboxStatus.Pending;
}

public static class OutboxStatus
{
    public const string Pending = "Pending";
    public const string Published = "Published";
    public const string Failed = "Failed";
}
