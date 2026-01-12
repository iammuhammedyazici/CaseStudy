using ECommerce.Contracts.Common;
using ECommerce.Notification.Domain.Models;

namespace ECommerce.Notification.Application.Abstractions.Services;

/// <summary>
/// Main notification service that orchestrates sending notifications through different channels
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends a notification through the specified channel
    /// </summary>
    /// <param name="request">The notification request containing channel, recipient, and content</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> SendAsync(NotificationRequest request, CancellationToken cancellationToken = default);
}
