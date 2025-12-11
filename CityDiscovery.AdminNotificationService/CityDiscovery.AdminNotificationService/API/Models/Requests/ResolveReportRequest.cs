namespace CityDiscovery.AdminNotificationService.API.Models.Requests
{
    public class ResolveReportRequest
    {
        public Guid AdminUserId { get; set; }
        public string NewStatus { get; set; } = default!; // "Resolved" veya "Rejected"
    }
}
