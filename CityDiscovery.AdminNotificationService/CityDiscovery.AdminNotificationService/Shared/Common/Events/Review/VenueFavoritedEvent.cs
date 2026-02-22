namespace CityDiscovery.ReviewService.ReviewService.Shared.Events.Venue
{
    public class VenueFavoritedEvent
    {
        public Guid VenueId { get; set; }
        public Guid UserId { get; set; }       // Favorileyen kişi
        public Guid OwnerUserId { get; set; }  // Bildirim bu kişiye gidecek
        public DateTime FavoritedAt { get; set; } = DateTime.UtcNow;
    }
}