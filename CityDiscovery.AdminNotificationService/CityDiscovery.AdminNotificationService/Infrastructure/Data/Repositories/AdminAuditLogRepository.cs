using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Domain.Entities;
using CityDiscovery.AdminNotificationService.Infrastructure.Data.Context;

namespace CityDiscovery.AdminNotificationService.Infrastructure.Data.Repositories
{
    public class AdminAuditLogRepository : IAdminAuditLogRepository
    {
        private readonly AdminNotificationDbContext _context;

        public AdminAuditLogRepository(AdminNotificationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AdminAuditLog log, CancellationToken cancellationToken = default)
        {
            await _context.AdminAuditLogs.AddAsync(log, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
