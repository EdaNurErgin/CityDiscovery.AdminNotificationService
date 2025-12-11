namespace CityDiscovery.AdminNotificationService.Shared.Common.Events.Venue
{
    public class VenueCreatedEvent
    {
        public Guid VenueId { get; set; }
        public Guid OwnerUserId { get; set; }
        public string Name { get; set; }
        public bool IsApproved { get; set; }
    }
}