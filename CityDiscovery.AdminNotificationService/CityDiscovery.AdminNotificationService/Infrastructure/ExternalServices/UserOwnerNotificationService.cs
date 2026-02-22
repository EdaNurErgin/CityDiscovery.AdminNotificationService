using CityDiscovery.AdminNotificationService.Application.Common.Models;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.DTOs;
using CityDiscovery.AdminNotificationService.Application.Interfaces.ExternalServices;
using CityDiscovery.AdminNotificationService.Domain.Entities;
using CityDiscovery.AdminNotificationService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CityDiscovery.AdminNotificationService.Infrastructure.ExternalServices
{
    // Interface'i de sadece bu 3 metodu içerecek şekilde güncellemeyi unutma

    public class UserOwnerNotificationService : IUserOwnerNotificationService
    {
        private readonly AdminNotificationDbContext _dbContext;

        // Artık HttpClient'lara veya SignalR Hub'a burada ihtiyacımız yok, 
        // çünkü Hub'ı Consumer'ların içinde kullanıyoruz.
        public UserOwnerNotificationService(AdminNotificationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<NotificationDto>> GetNotificationsAsync(string category, Guid recipientId, bool unreadOnly, int page, int pageSize)
        {
            var query = _dbContext.Set<Notification>().Where(n => n.UserId == recipientId);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(n => n.Type.StartsWith(category + "_"));

            if (unreadOnly)
                query = query.Where(n => !n.IsRead);

            var totalCount = await query.CountAsync();
            var items = await query.OrderByDescending(n => n.CreatedAt)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PagedResult<NotificationDto>
            {
                Items = items.Select(NotificationDto.FromEntity).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<int> GetUnreadCountAsync(string category, Guid recipientId)
        {
            var query = _dbContext.Set<Notification>().Where(n => n.UserId == recipientId && !n.IsRead);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(n => n.Type.StartsWith(category + "_"));

            return await query.CountAsync();
        }

        public async Task MarkAsReadAsync(Guid notificationId, Guid recipientId)
        {
            var notification = await _dbContext.Set<Notification>()
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == recipientId);

            if (notification != null)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}