using ECommerce.Order.Application.Orders.Queries.GetOrder;
using ECommerce.Order.Domain;

namespace ECommerce.Order.Application.Orders.Dtos;

public class OrderResponse
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public List<OrderItemResponse> Items { get; init; } = new();

    public string Source { get; init; } = string.Empty;
    public string? ExternalOrderId { get; init; }
    public string? ExternalSystemCode { get; init; }
    public AddressDto? ShippingAddress { get; init; }
    public AddressDto? BillingAddress { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public DateTime? CancelledAt { get; init; }
    public string? CancellationReason { get; init; }
    public string? CustomerNote { get; init; }

    public static OrderResponse FromDto(GetOrderDto dto)
    {
        return new OrderResponse
        {
            Id = dto.Id,
            UserId = dto.UserId,
            TotalAmount = dto.TotalAmount,
            Status = dto.Status.ToString(),
            CreatedAt = dto.CreatedAt,
            Items = dto.Items.Select(OrderItemResponse.FromDto).ToList(),
            Source = dto.Source.ToString(),
            ExternalOrderId = dto.ExternalOrderId,
            ExternalSystemCode = dto.ExternalSystemCode,
            ShippingAddress = dto.ShippingAddress,
            BillingAddress = dto.BillingAddress,
            UpdatedAt = dto.UpdatedAt,
            CancelledAt = dto.CancelledAt,
            CancellationReason = dto.CancellationReason,
            CustomerNote = dto.CustomerNote
        };
    }
}
