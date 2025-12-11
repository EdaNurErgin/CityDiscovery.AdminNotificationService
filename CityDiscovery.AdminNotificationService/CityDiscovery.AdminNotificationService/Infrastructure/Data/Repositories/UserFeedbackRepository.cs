using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Domain.Entities;
using CityDiscovery.AdminNotificationService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CityDiscovery.AdminNotificationService.Infrastructure.Data.Repositories
{
    public class UserFeedbackRepository : IUserFeedbackRepository
    {
        private readonly AdminNotificationDbContext _context;

        public UserFeedbackRepository(AdminNotificationDbContext context)
        {
            _context = context;
        }

        public async Task<UserFeedback?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.UserFeedbacks
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task AddAsync(UserFeedback feedback, CancellationToken cancellationToken = default)
        {
            await _context.UserFeedbacks.AddAsync(feedback, cancellationToken);
        }

        public Task UpdateAsync(UserFeedback feedback, CancellationToken cancellationToken = default)
        {
            _context.UserFeedbacks.Update(feedback);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<UserFeedback>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserFeedbacks
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<UserFeedback>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
        {
            return await _context.UserFeedbacks
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default)
        {
            return await _context.UserFeedbacks.CountAsync(cancellationToken);
        }

    }
}
