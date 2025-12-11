using CityDiscovery.AdminNotificationService.Domain.Entities;

namespace CityDiscovery.AdminNotificationService.Application.Features.Notifications.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = default!;
        public string? Payload { get; set; }
        public Guid UserId { get; set; }
        public Guid? ActorUserId { get; set; }
        public string? TargetType { get; set; }
        public Guid? TargetId { get; set; }
        public string? Route { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public static NotificationDto FromEntity(Notification entity)
        {
            return new NotificationDto
            {
                Id = entity.Id,
                Type = entity.Type,
                Payload = entity.Payload,
                UserId = entity.UserId,
                ActorUserId = entity.ActorUserId,
                TargetType = entity.TargetType,
                TargetId = entity.TargetId,
                Route = entity.Route,
                IsRead = entity.IsRead,
                ReadAt = entity.ReadAt,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
