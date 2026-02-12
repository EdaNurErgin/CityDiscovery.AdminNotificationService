namespace CityDiscovery.ReviewService.ReviewService.Shared.Events.Review
{
    public class ReviewDeletedEvent
    {
        public Guid ReviewId { get; set; }
        public Guid VenueId { get; set; }
    }
}