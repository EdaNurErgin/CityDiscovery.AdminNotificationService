using CityDiscovery.AdminNotificationService.Application.Common.Models;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.DTOs;
using CityDiscovery.AdminNotificationService.Application.Features.UserOwnerNotifications.DTOs;

namespace CityDiscovery.AdminNotificationService.Application.Interfaces.ExternalServices
{
    public interface IUserOwnerNotificationService
    {
    
            Task<PagedResult<NotificationDto>> GetNotificationsAsync(string category, Guid recipientId, bool unreadOnly, int page, int pageSize);
            Task<int> GetUnreadCountAsync(string category, Guid recipientId);
            Task MarkAsReadAsync(Guid notificationId, Guid recipientId);
        
    }
}
