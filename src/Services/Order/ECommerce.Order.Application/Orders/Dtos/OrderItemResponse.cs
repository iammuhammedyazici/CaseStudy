using ECommerce.Order.Application.Orders.Queries.GetOrder;

namespace ECommerce.Order.Application.Orders.Dtos;

public class OrderItemResponse
{
    public Guid ProductId { get; set; }
    public Guid VariantId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public static OrderItemResponse FromDto(OrderItemDto dto)
    {
        return new OrderItemResponse
        {
            ProductId = dto.ProductId,
            VariantId = dto.VariantId,
            Quantity = dto.Quantity,
            UnitPrice = dto.UnitPrice
        };
    }
}
