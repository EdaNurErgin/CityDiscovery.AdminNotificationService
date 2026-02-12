using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Domain.Entities;
using MassTransit;
// ReviewService namespace'i
using CityDiscovery.ReviewService.ReviewService.Shared.Events.Review;

namespace CityDiscovery.AdminNotificationService.Infrastructure.MessageBus.Consumers
{
    public class ReviewCreatedConsumer : IConsumer<ReviewCreatedEvent>
    {
        private readonly INotificationRepository _notificationRepository;

        public ReviewCreatedConsumer(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task Consume(ConsumeContext<ReviewCreatedEvent> context)
        {
            var msg = context.Message;

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = msg.VenueOwnerId, // Eventte mevcut
                Type = "ReviewAdded",
                ActorUserId = msg.UserId, // ReviewService'de yorum yapan 'UserId'
                Payload = $"{{\"venueId\":\"{msg.VenueId}\",\"rating\":{msg.Rating}}}",
                TargetType = "Venue",
                TargetId = msg.VenueId,
                Route = $"/venues/{msg.VenueId}/reviews",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
            await _notificationRepository.SaveChangesAsync();
        }
    }
}