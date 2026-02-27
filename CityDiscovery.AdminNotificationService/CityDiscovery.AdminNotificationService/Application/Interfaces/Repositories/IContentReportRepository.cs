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
        
        Task<(List<ContentReport> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            string? status, // Opsiyonel: 'Open', 'Resolved' vs. filtrelemek için
            CancellationToken cancellationToken = default);
        Task<List<ContentReport>> GetByReportingUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
