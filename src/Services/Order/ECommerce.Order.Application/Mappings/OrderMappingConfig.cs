using ECommerce.Contracts.Common;
using ECommerce.Order.Application.Orders.Queries.GetOrder;
using ECommerce.Order.Domain;
using Mapster;

namespace ECommerce.Order.Application.Mappings;

public class OrderMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Domain.Order, GetOrderDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.UserId, src => src.UserId)
            .Map(dest => dest.TotalAmount, src => src.TotalAmount)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.Items, src => src.Items)
            .Map(dest => dest.Source, src => src.Source)
            .Map(dest => dest.ExternalOrderId, src => src.ExternalOrderId)
            .Map(dest => dest.ExternalSystemCode, src => src.ExternalSystemCode)
            .Map(dest => dest.ShippingAddressId, src => src.ShippingAddressId)
            .Map(dest => dest.BillingAddressId, src => src.BillingAddressId)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt)
            .Map(dest => dest.CancelledAt, src => src.CancelledAt)
            .Map(dest => dest.CancellationReason, src => src.CancellationReason)
            .Map(dest => dest.CustomerNote, src => src.CustomerNote);

        config.NewConfig<OrderItem, OrderItemDto>()
            .Map(dest => dest.ProductId, src => GuidHelper.ToGuid(src.ProductId, "PROD"))
            .Map(dest => dest.VariantId, src => GuidHelper.ToGuid(src.VariantId, "VAR"))
            .Map(dest => dest.Quantity, src => src.Quantity)
            .Map(dest => dest.UnitPrice, src => src.UnitPrice);

        config.NewConfig<GetOrderDto, Orders.Dtos.OrderResponse>()
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.Source, src => src.Source.ToString());

        config.NewConfig<OrderItemDto, Orders.Dtos.OrderItemResponse>();
    }
}
