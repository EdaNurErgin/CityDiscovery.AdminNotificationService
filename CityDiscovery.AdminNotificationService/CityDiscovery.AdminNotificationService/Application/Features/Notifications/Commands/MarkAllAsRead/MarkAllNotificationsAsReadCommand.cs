using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.MarkAllAsRead
{
    public class MarkAllNotificationsAsReadCommand : IRequest
    {
        public Guid UserId { get; set; }
    }
}
