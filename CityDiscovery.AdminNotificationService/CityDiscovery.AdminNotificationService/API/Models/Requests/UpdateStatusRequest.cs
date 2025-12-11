namespace CityDiscovery.AdminNotificationService.API.Models.Requests
{
    public class UpdateStatusRequest
    {
        public Guid AdminUserId { get; set; }
        public string NewStatus { get; set; } = default!;
    }
}
