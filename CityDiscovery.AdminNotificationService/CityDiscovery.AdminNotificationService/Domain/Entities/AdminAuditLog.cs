using CityDiscovery.AdminNotificationService.Domain.Common;

namespace CityDiscovery.AdminNotificationService.Domain.Entities
{
    public class AdminAuditLog : BaseEntity
    {
        public Guid AdminUserId { get; set; }
        public string Action { get; set; } = default!;         // 'ApproveVenue','RejectVenue','DeleteComment',...
        public string? TargetType { get; set; }                // 'Venue','Post','Comment','User','Report'
        public Guid? TargetId { get; set; }
        public string? Details { get; set; }
    }
}
