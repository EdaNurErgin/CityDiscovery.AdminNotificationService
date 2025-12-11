using CityDiscovery.AdminNotificationService.Domain.Entities;

namespace CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories
{
    public interface IUserFeedbackRepository
    {
        Task<UserFeedback?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(UserFeedback feedback, CancellationToken cancellationToken = default);
        Task UpdateAsync(UserFeedback feedback, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<List<UserFeedback>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<UserFeedback>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);

        Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);

    }
}
