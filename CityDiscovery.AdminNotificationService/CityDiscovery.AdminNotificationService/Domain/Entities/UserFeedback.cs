using CityDiscovery.AdminNotificationService.Domain.Common;

namespace CityDiscovery.AdminNotificationService.Domain.Entities
{
    public class UserFeedback : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Type { get; set; } = default!;           // 'Feedback','Complaint','Suggestion','Bug'
        public string? Subject { get; set; }
        public string Message { get; set; } = default!;
        public string Status { get; set; } = "Open";           // 'Open','InProgress','Resolved','Closed'
        public DateTime? ResolvedAt { get; set; }
    }
}
