using MassTransit;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Shared.Common.Events.Social;

namespace CityDiscovery.AdminNotificationService.Infrastructure.MessageBus.Consumers
{
    public class PostDeletedConsumer : IConsumer<PostDeletedEvent>
    {
        private readonly INotificationRepository _notificationRepository;

        public PostDeletedConsumer(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task Consume(ConsumeContext<PostDeletedEvent> context)
        {
            // Silinen post'a ait bildirimleri temizle
            await _notificationRepository.DeleteByTargetIdAsync(context.Message.PostId, "Post");
        }
    }
}