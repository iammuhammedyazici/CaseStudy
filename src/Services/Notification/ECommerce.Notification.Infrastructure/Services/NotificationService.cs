using ECommerce.Contracts.Common;
using ECommerce.Notification.Application.Abstractions.Persistence;
using ECommerce.Notification.Application.Abstractions.Services;
using ECommerce.Notification.Domain;
using ECommerce.Notification.Domain.Enums;
using ECommerce.Notification.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ECommerce.Notification.Infrastructure.Services;

/// <summary>
/// Main notification service that orchestrates sending notifications through different channels
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly INotificationRepository _repository;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IEmailService emailService,
        ISmsService smsService,
        INotificationRepository repository,
        ILogger<NotificationService> logger)
    {
        _emailService = emailService;
        _smsService = smsService;
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result> SendAsync(NotificationRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Sending {Channel} notification to {Recipient}",
            request.Channel, request.Recipient);

        Result result = request.Channel switch
        {
            NotificationChannel.Email => await _emailService.SendEmailAsync(
                request.Recipient,
                request.Subject ?? "Notification",
                request.Content,
                cancellationToken),

            NotificationChannel.Sms => await _smsService.SendSmsAsync(
                request.Recipient,
                request.Content,
                cancellationToken),

            _ => Result.Failure($"Unsupported notification channel: {request.Channel}")
        };

        var log = new NotificationLog
        {
            Id = Guid.NewGuid(),
            OrderId = request.OrderId ?? Guid.Empty,
            Channel = request.Channel.ToString().ToLowerInvariant(),
            Recipient = request.Recipient,
            Content = request.Content,
            Status = result.IsSuccess ? "Sent" : "Failed",
            CreatedAtUtc = DateTime.UtcNow
        };

        _repository.AddNotification(log);

        return result;
    }
}
