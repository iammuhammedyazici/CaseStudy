namespace ECommerce.Order.Application.Orders.Dtos;

public record AddressDto(
    string FullName,
    string Phone,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string PostalCode,
    string Country
);
