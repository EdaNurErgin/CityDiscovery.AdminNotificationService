// Shared.Common/Events/Review/VenueRatingUpdatedEvent.cs
namespace CityDiscovery.VenueService.VenuesService.Shared.Common.Events.Review;
public class VenueRatingUpdatedEvent
{
    public Guid VenueId { get; set; }
    public double NewAverageRating { get; set; }
    public int TotalReviewCount { get; set; }
}