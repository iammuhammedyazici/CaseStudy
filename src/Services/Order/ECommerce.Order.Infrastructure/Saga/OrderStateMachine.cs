using ECommerce.Contracts;
using MassTransit;

namespace ECommerce.Order.Infrastructure.Saga;

/// <summary>
/// Saga State Machine for managing the Order lifecycle.
/// Orchestrates the flow: OrderCreated -> StockReserved -> Confirmed OR StockFailed -> Rejected.
/// </summary>
public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderCreated, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => StockReserved, x => x.CorrelateById(m => m.Message.OrderId));
        Event(() => StockFailed, x => x.CorrelateById(m => m.Message.OrderId));

        Initially(
            When(OrderCreated)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.CreatedAt = DateTime.UtcNow;
                })
                .TransitionTo(Submitted)
        );

        During(Submitted,
            When(StockReserved)
                .Then(context => context.Saga.UpdatedAt = DateTime.UtcNow)
                .TransitionTo(Confirmed),
            When(StockFailed)
                .Then(context => context.Saga.UpdatedAt = DateTime.UtcNow)
                .TransitionTo(Rejected)
        );
    }

    public State Submitted { get; private set; } = null!;
    public State Confirmed { get; private set; } = null!;
    public State Rejected { get; private set; } = null!;

    public Event<OrderCreatedEvent> OrderCreated { get; private set; } = null!;
    public Event<StockReservedEvent> StockReserved { get; private set; } = null!;
    public Event<StockReservationFailedEvent> StockFailed { get; private set; } = null!;
}
