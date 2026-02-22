using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Application.Interfaces.External;
using CityDiscovery.AdminNotificationService.Domain.Entities;
using CityDiscovery.SocialService.SocialServiceShared.Common.Events.Social;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using CityDiscovery.AdminNotificationService.API.Hubs;
using System.Text.Json;

namespace CityDiscovery.AdminNotificationService.Infrastructure.MessageBus.Consumers
{
    public class PostLikedConsumer : IConsumer<PostLikedEvent>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IIdentityServiceClient _identityServiceClient;
        private readonly IHubContext<UserNotificationHub> _userHubContext;
        private readonly ILogger<PostLikedConsumer> _logger;

        public PostLikedConsumer(
            INotificationRepository notificationRepository,
            IIdentityServiceClient identityServiceClient,
            IHubContext<UserNotificationHub> userHubContext,
            ILogger<PostLikedConsumer> logger)
        {
            _notificationRepository = notificationRepository;
            _identityServiceClient = identityServiceClient;
            _userHubContext = userHubContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PostLikedEvent> context)
        {
            var msg = context.Message;

            if (msg.PostAuthorUserId == msg.UserId)
                return;

            if (msg.PostAuthorUserId == Guid.Empty)
            {
                _logger.LogWarning("PostAuthorUserId boş geldi. PostId: {PostId}", msg.PostId);
                return;
            }

            // Beğenen kişinin adını çek
            var actor = await _identityServiceClient.GetUserAsync(msg.UserId);
            var actorName = actor?.UserName ?? "Bir kullanıcı";

            var payloadJson = JsonSerializer.Serialize(new
            {
                PostId = msg.PostId,
                LikerUserId = msg.UserId,
                LikerUserName = actorName,
                LikerAvatarUrl = actor?.AvatarUrl,
                LikedAt = msg.LikedAt
            });

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = msg.PostAuthorUserId,
                Type = "PostLiked",
                Message = $"{actorName} gönderini beğendi.",
                ActorUserId = msg.UserId,
                Payload = payloadJson,
                TargetType = "Post",
                TargetId = msg.PostId,
                Route = $"/posts/{msg.PostId}",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
            await _notificationRepository.SaveChangesAsync();

            try
            {
                await _userHubContext.Clients
                    .User(msg.PostAuthorUserId.ToString())
                    .SendAsync("ReceiveNotification", new
                    {
                        Id = notification.Id,
                        Type = notification.Type,
                        Message = notification.Message,
                        Route = notification.Route,
                        ActorUserId = msg.UserId,
                        ActorUserName = actorName,
                        ActorAvatarUrl = actor?.AvatarUrl,
                        PostId = msg.PostId,
                        CreatedAt = notification.CreatedAt
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR gönderimi başarısız. UserId: {UserId}", msg.PostAuthorUserId);
            }
        }
    }
}