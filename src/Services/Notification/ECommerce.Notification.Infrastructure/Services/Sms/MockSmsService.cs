using ECommerce.Contracts.Common;
using ECommerce.Notification.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace ECommerce.Notification.Infrastructure.Services.Sms;

/// <summary>
/// Mock SMS service for development and testing
/// Logs SMS details instead of sending actual messages
/// </summary>
public class MockSmsService : ISmsService
{
    private readonly ILogger<MockSmsService> _logger;

    public MockSmsService(ILogger<MockSmsService> logger)
    {
        _logger = logger;
    }

    public Task<Result> SendSmsAsync(string to, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[MOCK SMS] To: {To}, Message: {Message}",
            to, message);

        return Task.FromResult(Result.Success());
    }
}
