using MassTransit;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Shared.Common.Events.Review;

namespace CityDiscovery.AdminNotificationService.Infrastructure.MessageBus.Consumers
{
    public class ReviewDeletedConsumer : IConsumer<ReviewDeletedEvent>
    {
        private readonly INotificationRepository _notificationRepository;

        public ReviewDeletedConsumer(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task Consume(ConsumeContext<ReviewDeletedEvent> context)
        {
            // Silinen yoruma ait "Yeni Yorum" bildirimlerini temizle
            await _notificationRepository.DeleteByTargetIdAsync(context.Message.ReviewId, "Review");
        }
    }
}