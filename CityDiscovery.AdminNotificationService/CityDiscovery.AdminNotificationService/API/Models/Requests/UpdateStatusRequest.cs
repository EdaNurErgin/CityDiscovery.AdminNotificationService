using System.Text.Json.Serialization;

namespace CityDiscovery.AdminNotificationService.API.Models.Requests
{
    public class UpdateStatusRequest
    {
        [JsonIgnore]
        public Guid AdminUserId { get; set; }
        public string NewStatus { get; set; } = default!;
    }
}
