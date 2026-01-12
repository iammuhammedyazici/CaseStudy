using ECommerce.Notification.Application.Abstractions.Services;
using ECommerce.Notification.Domain.Enums;
using ECommerce.Notification.Domain.Models;
using MediatR;

namespace ECommerce.Notification.Application.Notifications.Commands.SendNotification;

/// <summary>
/// Handler for sending notifications through various channels
/// </summary>
public class SendNotificationHandler : IRequestHandler<SendNotificationCommand>
{
    private readonly INotificationService _notificationService;

    public SendNotificationHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var notificationRequest = new NotificationRequest
        {
            Channel = NotificationChannel.Email,
            Recipient = "user@example.com",
            Subject = $"Order {request.OrderId} Update",
            Content = $"Your order {request.OrderId} stock status: {request.Status}",
            OrderId = request.OrderId
        };

        await _notificationService.SendAsync(notificationRequest, cancellationToken);
    }
}
