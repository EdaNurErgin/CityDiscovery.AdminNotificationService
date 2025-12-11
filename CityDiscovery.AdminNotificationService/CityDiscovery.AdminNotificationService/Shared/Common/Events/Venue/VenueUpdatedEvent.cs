// Shared.Common/Events/Venue/VenueUpdatedEvent.cs
public class VenueUpdatedEvent
{
    public Guid VenueId { get; set; }
    public string NewName { get; set; }
    public string NewCoverPhotoUrl { get; set; }
}