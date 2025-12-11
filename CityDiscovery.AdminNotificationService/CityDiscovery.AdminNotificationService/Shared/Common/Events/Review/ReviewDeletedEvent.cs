namespace CityDiscovery.AdminNotificationService.Shared.Common.Events.Review
{
    public class ReviewDeletedEvent
    {
        public Guid ReviewId { get; set; }
        public Guid VenueId { get; set; }
    }
}