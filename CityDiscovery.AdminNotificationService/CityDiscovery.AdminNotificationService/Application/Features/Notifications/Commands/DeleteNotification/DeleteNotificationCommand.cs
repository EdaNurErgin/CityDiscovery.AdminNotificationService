using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.DeleteNotification
{
    public class DeleteNotificationCommand : IRequest
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; } // Güvenlik kontrolü için
    }
}