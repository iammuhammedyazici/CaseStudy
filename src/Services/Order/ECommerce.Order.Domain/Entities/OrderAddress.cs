namespace ECommerce.Order.Domain;

public class OrderAddress
{
    public Guid Id { get; set; }
    public Guid? OrderId { get; set; }
    public string AddressType { get; set; } = string.Empty; // "Shipping" or "Billing"

    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
