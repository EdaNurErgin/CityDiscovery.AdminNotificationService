// Shared.Common/Events/Review/VenueRatingUpdatedEvent.cs
public class VenueRatingUpdatedEvent
{
    public Guid VenueId { get; set; }
    public double NewAverageRating { get; set; }
    public int TotalReviewCount { get; set; }
}