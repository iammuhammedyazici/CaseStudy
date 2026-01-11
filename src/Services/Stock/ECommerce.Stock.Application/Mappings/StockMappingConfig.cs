using ECommerce.Contracts.Common;
using ECommerce.Stock.Application.Stock.Queries.GetStock;
using Mapster;

namespace ECommerce.Stock.Application.Mappings;

public class StockMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Domain.StockItem, GetStockResult>()
            .Map(dest => dest.VariantId, src => GuidHelper.ToGuid(src.VariantId, "VAR"))
            .Map(dest => dest.ProductId, src => GuidHelper.ToGuid(src.ProductId, "PROD"))
            .Map(dest => dest.AvailableQuantity, src => src.AvailableQuantity)
            .Map(dest => dest.ReservedQuantity, src => src.ReservedQuantity)
            .Map(dest => dest.ActualAvailable, src => src.ActualAvailable);
    }
}
