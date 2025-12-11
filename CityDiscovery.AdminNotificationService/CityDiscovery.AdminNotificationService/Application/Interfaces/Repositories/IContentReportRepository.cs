using CityDiscovery.AdminNotificationService.Domain.Entities;

namespace CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories
{
    public interface IContentReportRepository
    {
        Task<ContentReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(ContentReport report, CancellationToken cancellationToken = default);
        Task UpdateAsync(ContentReport report, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<int> GetOpenCountAsync(CancellationToken cancellationToken = default);

    }
}
