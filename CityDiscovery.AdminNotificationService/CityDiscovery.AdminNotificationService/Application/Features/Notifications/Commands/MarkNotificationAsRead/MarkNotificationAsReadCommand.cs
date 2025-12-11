using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.MarkAsRead
{
    public class MarkNotificationAsReadCommand : IRequest
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }   // güvenlik için: sadece kendi bildirimini okuyabilsin
    }
}
