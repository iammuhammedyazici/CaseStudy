using MassTransit;

namespace ECommerce.Order.Infrastructure.Entities;

public class OutboxState
{
    public Guid OutboxId { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Delivered { get; set; }

    public long? LastSequenceNumber { get; set; }

    public byte[]? RowVersion { get; set; }
}
