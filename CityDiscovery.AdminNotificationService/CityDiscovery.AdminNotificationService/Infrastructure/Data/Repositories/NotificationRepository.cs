using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Domain.Entities;
using CityDiscovery.AdminNotificationService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CityDiscovery.AdminNotificationService.Infrastructure.Data.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AdminNotificationDbContext _context;

        public NotificationRepository(AdminNotificationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
        {
            await _context.Notifications.AddAsync(notification, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<Notification> Items, int TotalCount)>
            GetPagedForUserAsync(Guid userId, int page, int pageSize, bool unreadOnly, CancellationToken cancellationToken = default)
        {
            if (page < 1) page = 1;
            if (pageSize <= 0) pageSize = 20;

            var query = _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .AsQueryable();

            if (unreadOnly)
                query = query.Where(n => !n.IsRead);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync(cancellationToken);
        }

        public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
        }

        public async Task MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var notification = await GetByIdAsync(id, cancellationToken);
            if (notification == null) return;

            if (!notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                _context.Notifications.Update(notification);
            }
        }

        public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync(cancellationToken);

            if (!notifications.Any()) return;

            var now = DateTime.UtcNow;

            foreach (var n in notifications)
            {
                n.IsRead = true;
                n.ReadAt = now;
            }

            _context.Notifications.UpdateRange(notifications);
        }

        public async Task AddRangeAsync(IEnumerable<Notification> notifications)
        {
            await _context.Notifications.AddRangeAsync(notifications);
            // Eğer SaveChanges repository içinde yapılıyorsa:
            await _context.SaveChangesAsync();
        }

        // Mevcut koda ekle:
        public async Task DeleteByTargetIdAsync(Guid targetId, string targetType)
        {
            // EF Core ExecuteDeleteAsync (Toplu silme - Performanslı yöntem)
            await _context.Notifications
                .Where(n => n.TargetId == targetId && n.TargetType == targetType)
                .ExecuteDeleteAsync();

            // Eğer eski EF Core kullanıyorsan veya ExecuteDeleteAsync yoksa:
            /*
            var notifications = await _context.Notifications
                .Where(n => n.TargetId == targetId && n.TargetType == targetType)
                .ToListAsync();

            _context.Notifications.RemoveRange(notifications);
            await _context.SaveChangesAsync();
            */
        }
    }
}
