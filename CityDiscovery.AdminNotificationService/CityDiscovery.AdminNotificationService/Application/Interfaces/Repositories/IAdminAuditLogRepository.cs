using CityDiscovery.AdminNotificationService.Domain.Entities;

namespace CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories
{
    public interface IAdminAuditLogRepository
    {
        Task AddAsync(AdminAuditLog log, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
