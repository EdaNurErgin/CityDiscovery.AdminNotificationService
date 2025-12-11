using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Domain.Entities;
using CityDiscovery.AdminNotificationService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CityDiscovery.AdminNotificationService.Infrastructure.Data.Repositories
{
    public class ContentReportRepository : IContentReportRepository
    {
        private readonly AdminNotificationDbContext _context;

        public ContentReportRepository(AdminNotificationDbContext context)
        {
            _context = context;
        }

        public async Task<ContentReport?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.ContentReports
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task AddAsync(ContentReport report, CancellationToken cancellationToken = default)
        {
            await _context.ContentReports.AddAsync(report, cancellationToken);
        }

        public Task UpdateAsync(ContentReport report, CancellationToken cancellationToken = default)
        {
            _context.ContentReports.Update(report);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> GetOpenCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.ContentReports
                .Where(r => r.Status == "Open")
                .CountAsync(cancellationToken);
        }

        // Sınıfın içine ekleyin:
        public async Task<(List<ContentReport> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            string? status,
            CancellationToken cancellationToken = default)
        {
            var query = _context.ContentReports.AsQueryable();

            // Eğer status parametresi doluysa filtrele
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(r => r.CreatedAt) // En yeniden eskiye
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
