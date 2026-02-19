
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
        // Endpoint: GET /api/Users/{id}
        // ================================
        public async Task<UserBasicInfo?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                // UsersController -> [Route("api/[controller]")] -> api/Users
                var response = await _httpClient.GetAsync($"/api/Users/{userId}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadFromJsonAsync<UserBasicInfo>(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                // Loglama eklenebilir
                Console.WriteLine($"[IdentityServiceClient] GetUserAsync Error: {ex.Message}");
                return null;
            }
        }

        // ================================
        // ✔️ Çoklu kullanıcı bilgisi getir
        // Endpoint: POST /api/Users/bulk
        // Body Expectation: [ "guid1", "guid2" ] (List<Guid>)
        // ================================
        public async Task<List<UserBasicInfo>> GetUsersAsync(List<Guid> userIds, CancellationToken cancellationToken = default)
        {
            try
            {
                // DÜZELTME 1: Endpoint ismi 'batch' değil 'bulk' olmalı (Controller ile uyumlu)
                // DÜZELTME 2: Body olarak 'new { userIds }' değil, direkt 'userIds' listesi gönderilmeli
                var response = await _httpClient.PostAsJsonAsync("/api/Users/bulk", userIds, cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return new List<UserBasicInfo>();

                var result = await response.Content.ReadFromJsonAsync<List<UserBasicInfo>>(cancellationToken: cancellationToken);

                return result ?? new List<UserBasicInfo>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IdentityServiceClient] GetUsersAsync Error: {ex.Message}");
                return new List<UserBasicInfo>();
            }
        }

        // ================================
        // ✔️ Role göre kullanıcıları getir
        // Endpoint: GET /api/Users/by-role/{role}
        // Kullanım: AdminNotificationService, bildirim göndereceği Adminleri bulmak için kullanır.
        // ================================
        public async Task<List<UserBasicInfo>> GetUsersByRoleAsync(string role)
        {
            try
            {
                // Identity Service -> UsersController -> [HttpGet("by-role/{role}")]
                var response = await _httpClient.GetAsync($"/api/Users/by-role/{role}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[IdentityServiceClient] GetUsersByRoleAsync Failed. Status: {response.StatusCode}");
                    return new List<UserBasicInfo>();
                }

                var result = await response.Content.ReadFromJsonAsync<List<UserBasicInfo>>();
                return result ?? new List<UserBasicInfo>();
            }
            catch (Exception ex)
            {
                // Identity servis kapalıysa veya hata varsa boş liste dönerek akışı bozmayız
                Console.WriteLine($"[IdentityServiceClient] GetUsersByRoleAsync Error: {ex.Message}");
                return new List<UserBasicInfo>();
            }
        }
    }
}