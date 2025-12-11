using System;

// DİKKAT: VenueService'deki namespace ile birebir aynı olmalı
namespace CityDiscovery.VenueService.VenuesService.Shared.Common.Events.Venue
{
    public class VenueApprovedEvent
    {
        // VenueService bu alanları gönderiyor
        public Guid Id { get; set; }
        public DateTime OccurredOn { get; set; }
        public Guid VenueId { get; set; }
        public Guid OwnerUserId { get; set; }
        public string VenueName { get; set; }
        public DateTime ApprovedAt { get; set; }
    }
}