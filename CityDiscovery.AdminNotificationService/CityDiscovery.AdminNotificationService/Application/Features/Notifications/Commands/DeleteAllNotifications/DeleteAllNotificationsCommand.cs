using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.DeleteAllNotifications
{
    public class DeleteAllNotificationsCommand : IRequest
    {
        public Guid UserId { get; set; }
    }
}