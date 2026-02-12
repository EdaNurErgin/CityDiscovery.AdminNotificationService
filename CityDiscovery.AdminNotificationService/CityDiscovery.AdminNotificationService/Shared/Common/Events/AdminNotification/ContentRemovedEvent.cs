namespace CityDiscovery.AdminNotificationService.Shared.Common.Events.AdminNotification
{
    public class ContentRemovedEvent
    {
        public Guid ContentId { get; set; }     // Silinecek şeyin ID'si
        public string ContentType { get; set; } // "Comment", "Venue", "Post" vb.
        public string Reason { get; set; }
    }
}
