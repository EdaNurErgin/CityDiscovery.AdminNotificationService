using MassTransit;
using System.Text.Json;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Domain.Entities;
using CityDiscovery.SocialService.SocialServiceShared.Common.Events.Social;

namespace CityDiscovery.AdminNotificationService.Infrastructure.MessageBus.Consumers
{
    public class CommentAddedConsumer : IConsumer<CommentAddedEvent>
    {
        private readonly INotificationRepository _notificationRepository;

        public CommentAddedConsumer(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task Consume(ConsumeContext<CommentAddedEvent> context)
        {
            var msg = context.Message;

            // Kendi postuna yorum yaptıysa bildirim atma
            if (msg.PostAuthorUserId == msg.CommenterUserId)
                return;

            var payloadJson = JsonSerializer.Serialize(new
            {
                PostId = msg.PostId,
                CommentId = msg.CommentId,
                CommentPreview = msg.CommentBody.Length > 50 ? msg.CommentBody.Substring(0, 50) + "..." : msg.CommentBody
            });

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = msg.PostAuthorUserId, // Post sahibine
                Type = "NewComment",
                ActorUserId = msg.CommenterUserId, // Yorum yapan kişi
                Payload = payloadJson,
                TargetType = "Post",
                TargetId = msg.PostId,
                Route = $"/posts/{msg.PostId}", // Post detay linki
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
        }
    }
}