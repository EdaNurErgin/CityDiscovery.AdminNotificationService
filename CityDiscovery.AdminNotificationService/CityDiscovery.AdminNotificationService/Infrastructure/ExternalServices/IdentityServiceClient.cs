using System.Net.Http.Json;
using CityDiscovery.AdminNotificationService.Application.Interfaces.External;
using CityDiscovery.AdminNotificationService.Application.Models;

namespace CityDiscovery.AdminNotificationService.Infrastructure.ExternalServices
{
    public class IdentityServiceClient : IIdentityServiceClient
    {
        private readonly HttpClient _httpClient;

        public IdentityServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ================================
        // ✔️ Tek bir kullanıcı bilgisi getir
        // GET /api/users/{id}
        // IdentityService'te endpoint'in nasıl olduğuna göre route'ı düzenle
        // ================================
        public async Task<UserBasicInfo?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/users/{userId}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<UserBasicInfo>(cancellationToken: cancellationToken);
            }
            catch
            {
                return null;
            }
        }

        // ================================
        // ✔️ Çoklu kullanıcı bilgisi getir
        // POST /api/users/batch
        // Body → { userIds: [] }
        // ================================
        public async Task<List<UserBasicInfo>> GetUsersAsync(List<Guid> userIds, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/users/batch", new { userIds }, cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return new List<UserBasicInfo>();

                var result = await response.Content.ReadFromJsonAsync<List<UserBasicInfo>>(cancellationToken: cancellationToken);

                return result ?? new List<UserBasicInfo>();
            }
            catch
            {
                return new List<UserBasicInfo>();
            }
        }
        public async Task<List<UserBasicInfo>> GetUsersByRoleAsync(string role)
        {
            // Identity Service: [HttpGet("api/users/by-role/{role}")]
            return await _httpClient.GetFromJsonAsync<List<UserBasicInfo>>($"/api/users/by-role/{role}");
        }
    }
}
