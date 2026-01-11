using System.ComponentModel.DataAnnotations;
using MassTransit;

namespace ECommerce.Order.Infrastructure.Saga;

public class OrderState : SagaStateMachineInstance
{
    [Key]
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;
    public Guid OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
