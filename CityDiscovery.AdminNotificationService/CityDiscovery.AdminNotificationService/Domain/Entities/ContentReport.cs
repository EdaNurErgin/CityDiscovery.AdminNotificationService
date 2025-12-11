using CityDiscovery.AdminNotificationService.Domain.Common;

namespace CityDiscovery.AdminNotificationService.Domain.Entities
{
    public class ContentReport : BaseEntity
    {
        public string ReportedType { get; set; } = default!;   // 'Post','Comment','Photo','Venue','Review'
        public Guid ReportedId { get; set; }
        public Guid ReportingUserId { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = "Open";           // 'Open','Resolved','Rejected'
        public DateTime? ResolvedAt { get; set; }
        public Guid? ResolvedByUserId { get; set; }
    }
}
