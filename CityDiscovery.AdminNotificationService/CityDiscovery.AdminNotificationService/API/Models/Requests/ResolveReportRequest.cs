using System.Text.Json.Serialization;

namespace CityDiscovery.AdminNotificationService.API.Models.Requests
{
    public class ResolveReportRequest
    {
        [JsonIgnore]
        public Guid AdminUserId { get; set; }
        public string NewStatus { get; set; } = default!; // "Resolved" veya "Rejected"
    }
}
