using ECommerce.Notification.Application.Abstractions.Persistence;
using ECommerce.Notification.Domain;
using MediatR;

namespace ECommerce.Notification.Application.Notifications.Commands.SendNotification;

public class SendNotificationHandler : IRequestHandler<SendNotificationCommand>
{
    private readonly INotificationRepository _repository;

    public SendNotificationHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public Task Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var log = new NotificationLog
        {
            Id = Guid.NewGuid(),
            OrderId = request.OrderId,
            Channel = "email",
            Recipient = "user@example.com",
            Content = $"Order {request.OrderId} stock status: {request.Status}",
            Status = "Sent",
            CreatedAtUtc = DateTime.UtcNow
        };

        _repository.AddNotification(log);
        return Task.CompletedTask;
    }
}
