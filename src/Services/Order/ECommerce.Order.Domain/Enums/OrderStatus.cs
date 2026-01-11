namespace ECommerce.Order.Domain;

public enum OrderStatus
{
    PendingStock = 0,
    Confirmed = 1,
    Rejected = 2,
    Cancelled = 3
}
