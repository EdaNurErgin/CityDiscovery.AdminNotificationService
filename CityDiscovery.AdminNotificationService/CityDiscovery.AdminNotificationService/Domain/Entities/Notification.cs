using CityDiscovery.AdminNotificationService.Domain.Common;

namespace CityDiscovery.AdminNotificationService.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Type { get; set; } = default!;       // 'PostLiked','ReviewAdded', ...
        public string Message { get; set; } = default!; // Bildirimin okunabilir metni
        public string? Payload { get; set; }               // Küçük JSON
        public Guid? ActorUserId { get; set; }             // Tetikleyen kullanıcı
        public string? TargetType { get; set; }            // 'Post','Review','Venue'
        public Guid? TargetId { get; set; }
        public string? Route { get; set; }                 // /venues/{id}/details
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
