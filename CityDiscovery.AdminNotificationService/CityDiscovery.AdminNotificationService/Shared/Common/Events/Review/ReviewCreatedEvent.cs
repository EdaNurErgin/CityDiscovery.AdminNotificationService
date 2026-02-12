using System;

// DİKKAT: ReviewService'deki namespace ile birebir aynı olmalı
namespace CityDiscovery.ReviewService.ReviewService.Shared.Events.Review
{
    public class ReviewCreatedEvent
    {
        public Guid ReviewId { get; set; }
        public Guid VenueId { get; set; }
        public Guid UserId { get; set; } // Yorum yapan kişi
        public int Rating { get; set; }
        public string Comment { get; set; }
        public Guid VenueOwnerId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}