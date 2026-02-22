namespace CityDiscovery.AdminNotificationService.Application.Features.UserOwnerNotifications.DTOs
{
    public class CreateVenueReviewNotificationRequest
    {
        public Guid VenueId { get; set; }
        public Guid ReviewId { get; set; }
        public Guid ReviewerUserId { get; set; }
    }
}
