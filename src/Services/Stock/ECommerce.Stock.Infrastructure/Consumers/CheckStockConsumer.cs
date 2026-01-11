using ECommerce.Contracts;
using ECommerce.Stock.Application.Abstractions.Persistence;
using MassTransit;

namespace ECommerce.Stock.Infrastructure.Consumers;

public class CheckStockConsumer : IConsumer<CheckStockRequest>
{
    private readonly IStockRepository _repository;

    public CheckStockConsumer(IStockRepository repository)
    {
        _repository = repository;
    }

    public async Task Consume(ConsumeContext<CheckStockRequest> context)
    {
        var unavailableItems = new List<UnavailableStockItem>();

        foreach (var item in context.Message.Items)
        {
            var stock = await _repository.GetStockItemAsync(item.VariantId, context.CancellationToken);

            if (stock == null)
            {
                unavailableItems.Add(new UnavailableStockItem(item.VariantId, item.Quantity, 0));
                continue;
            }

            var actualAvailable = stock.AvailableQuantity - stock.ReservedQuantity;

            if (actualAvailable < item.Quantity)
            {
                unavailableItems.Add(new UnavailableStockItem(item.VariantId, item.Quantity, actualAvailable));
            }
        }

        await context.RespondAsync(new CheckStockResponse(
            IsAvailable: unavailableItems.Count == 0,
            UnavailableItems: unavailableItems
        ));
    }
}
