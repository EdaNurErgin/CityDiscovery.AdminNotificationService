// Shared.Common/Events/Venue/VenueUpdatedEvent.cs
namespace CityDiscovery.VenuesService.Shared.Common.Events.Venue;
public class VenueUpdatedEvent
{
    public Guid VenueId { get; set; }
    public string NewName { get; set; }
    public string NewCoverPhotoUrl { get; set; }
}