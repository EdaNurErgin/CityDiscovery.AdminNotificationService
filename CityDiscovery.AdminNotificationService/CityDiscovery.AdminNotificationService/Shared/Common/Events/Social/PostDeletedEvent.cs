namespace CityDiscovery.AdminNotificationService.Shared.Common.Events.Social
{
    public class PostDeletedEvent
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; } // Post sahibi (Opsiyonel, silme işlemi için şart değil ama loglama için iyi olabilir)
    }
}