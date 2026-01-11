using MediatR;

namespace ECommerce.Notification.Application.Notifications.Commands.SendNotification;

public record SendNotificationCommand(Guid OrderId, string Status) : IRequest;
