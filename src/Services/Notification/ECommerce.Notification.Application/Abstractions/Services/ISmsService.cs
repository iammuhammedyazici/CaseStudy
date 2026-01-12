using ECommerce.Contracts.Common;

namespace ECommerce.Notification.Application.Abstractions.Services;

/// <summary>
/// Service for sending SMS notifications
/// </summary>
public interface ISmsService
{
    /// <summary>
    /// Sends an SMS
    /// </summary>
    /// <param name="to">Recipient phone number (E.164 format recommended)</param>
    /// <param name="message">SMS message content</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> SendSmsAsync(string to, string message, CancellationToken cancellationToken = default);
}
