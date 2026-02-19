using CityDiscovery.AdminNotificationService.Domain.Entities;

namespace CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        // 🔹 Yeni eklediğimiz metotlar:
        Task<(IReadOnlyList<Notification> Items, int TotalCount)>
        GetPagedForUserAsync(Guid userId, int page, int pageSize, bool unreadOnly, CancellationToken cancellationToken = default);

        Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task MarkAsReadAsync(Guid id, CancellationToken cancellationToken = default);

        Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<Notification> notifications);
        // Mevcut koda ekle:
        Task DeleteByTargetIdAsync(Guid targetId, string targetType);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteAllAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
