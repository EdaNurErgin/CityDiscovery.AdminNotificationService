using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CityDiscovery.AdminNotificationService.Application.Interfaces.External;
using CityDiscovery.AdminNotificationService.Application.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CityDiscovery.AdminNotificationService.Infrastructure.ExternalServices
{
    public class IdentityServiceClient : IIdentityServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public IdentityServiceClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        // Servis-to-servis çağrıları için kısa ömürlü token üretir
        // appsettings.json'daki aynı key/issuer/audience kullanılır
        private string GenerateServiceToken()
        {
            var key = _configuration["Jwt:Key"]!;
            var issuer = _configuration["Jwt:Issuer"]!;
            var audience = _configuration["Jwt:Audience"]!;

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("sub", "admin-notification-service"),
                new Claim(ClaimTypes.Role, "Admin"), // Admin rolü ile çağırıyoruz
                new Claim("service", "AdminNotificationService")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5), // Kısa ömürlü
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Her istekten önce token ekle
        private void SetAuthHeader()
        {
            var token = GenerateServiceToken();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        // ================================
        // Tek bir kullanıcı bilgisi getir
        // Endpoint: GET /api/Users/{id}
        // ================================
        public async Task<UserBasicInfo?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                SetAuthHeader();
                var response = await _httpClient.GetAsync($"/api/Users/{userId}", cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[IdentityServiceClient] GetUserAsync Failed. Status: {response.StatusCode}, UserId: {userId}");
                    return null;
                }

                return await response.Content.ReadFromJsonAsync<UserBasicInfo>(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IdentityServiceClient] GetUserAsync Error: {ex.Message}");
                return null;
            }
        }

        // ================================
        // Çoklu kullanıcı bilgisi getir
        // Endpoint: POST /api/Users/bulk
        // ================================
        public async Task<List<UserBasicInfo>> GetUsersAsync(List<Guid> userIds, CancellationToken cancellationToken = default)
        {
            try
            {
                SetAuthHeader();
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
        // Role göre kullanıcıları getir
        // Endpoint: GET /api/Users/by-role/{role}
        // ================================
        public async Task<List<UserBasicInfo>> GetUsersByRoleAsync(string role)
        {
            try
            {
                SetAuthHeader();
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
                Console.WriteLine($"[IdentityServiceClient] GetUsersByRoleAsync Error: {ex.Message}");
                return new List<UserBasicInfo>();
            }
        }
    }
}