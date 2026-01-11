using ECommerce.Contracts.Common;
using ECommerce.Product.Application.Common.Dtos;
using Mapster;

namespace ECommerce.Product.Application.Mappings;

public class ProductMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Domain.Entities.Product, ProductDto>()
            .Map(dest => dest.Id, src => GuidHelper.ToGuid(src.Id, "PROD"))
            .Map(dest => dest.Variants, src => src.Variants);

        config.NewConfig<Domain.Entities.ProductVariant, ProductVariantDto>()
            .Map(dest => dest.Id, src => GuidHelper.ToGuid(src.Id, "VAR"));
    }
}
