namespace ECommerce.Order.Application.Abstractions;

public interface ICurrentUserService
{
    string? UserId { get; }
    bool IsAdmin { get; }
}
