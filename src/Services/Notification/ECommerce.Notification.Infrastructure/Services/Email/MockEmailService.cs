using ECommerce.Contracts.Common;
using ECommerce.Notification.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace ECommerce.Notification.Infrastructure.Services.Email;

/// <summary>
/// Mock email service for development and testing
/// Logs email details instead of sending actual emails
/// </summary>
public class MockEmailService : IEmailService
{
    private readonly ILogger<MockEmailService> _logger;

    public MockEmailService(ILogger<MockEmailService> logger)
    {
        _logger = logger;
    }

    public Task<Result> SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[MOCK EMAIL] To: {To}, Subject: {Subject}, Body: {Body}",
            to, subject, body);

        return Task.FromResult(Result.Success());
    }
}
