using CityDiscovery.AdminNotificationService.Application.Models;

namespace CityDiscovery.AdminNotificationService.Application.Interfaces.External
{
    public interface IIdentityServiceClient
    {
        Task<UserBasicInfo?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<UserBasicInfo>> GetUsersAsync(List<Guid> userIds, CancellationToken cancellationToken = default);
        Task<List<UserBasicInfo>> GetUsersByRoleAsync(string role);
    }
}
