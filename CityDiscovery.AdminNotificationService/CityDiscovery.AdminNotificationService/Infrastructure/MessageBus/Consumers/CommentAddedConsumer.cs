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
    public class CommentAddedConsumer : IConsumer<CommentAddedEvent>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IIdentityServiceClient _identityServiceClient;
        private readonly IHubContext<UserNotificationHub> _userHubContext;
        private readonly ILogger<CommentAddedConsumer> _logger;

        public CommentAddedConsumer(
            INotificationRepository notificationRepository,
            IIdentityServiceClient identityServiceClient,
            IHubContext<UserNotificationHub> userHubContext,
            ILogger<CommentAddedConsumer> logger)
        {
            _notificationRepository = notificationRepository;
            _identityServiceClient = identityServiceClient;
            _userHubContext = userHubContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CommentAddedEvent> context)
        {
            var msg = context.Message;

            // Kendi postuna yorum yaptıysa bildirim atma
            if (msg.PostAuthorUserId == msg.UserId)
                return;

            if (msg.PostAuthorUserId == Guid.Empty)
            {
                _logger.LogWarning("PostAuthorUserId boş geldi. PostId: {PostId}", msg.PostId);
                return;
            }

            // Event içinde AuthorUserName zaten geliyor, boşsa IdentityService'e sor
            string actorName;
            string? actorAvatarUrl;

            if (!string.IsNullOrEmpty(msg.AuthorUserName))
            {
                actorName = msg.AuthorUserName;
                actorAvatarUrl = msg.AuthorAvatarUrl;
            }
            else
            {
                var actor = await _identityServiceClient.GetUserAsync(msg.UserId);
                actorName = actor?.UserName ?? "Bir kullanıcı";
                actorAvatarUrl = actor?.AvatarUrl;
            }

            // CommentBody → Content
            var commentPreview = string.IsNullOrEmpty(msg.Content)
                ? ""
                : (msg.Content.Length > 50 ? msg.Content.Substring(0, 50) + "..." : msg.Content);

            var payloadJson = JsonSerializer.Serialize(new
            {
                PostId = msg.PostId,
                CommentId = msg.CommentId,
                CommenterUserId = msg.UserId,
                CommenterUserName = actorName,
                CommenterAvatarUrl = actorAvatarUrl,
                CommentPreview = commentPreview
            });

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = msg.PostAuthorUserId,
                Type = "NewComment",
                Message = $"{actorName} gönderine yorum yaptı: \"{commentPreview}\"",
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

            _logger.LogInformation(
                "Yorum bildirimi kaydedildi. PostAuthor: {AuthorId}, Commenter: {CommenterId}",
                msg.PostAuthorUserId, msg.UserId);

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
                        ActorAvatarUrl = actorAvatarUrl,
                        PostId = msg.PostId,
                        CommentId = msg.CommentId,
                        CommentPreview = commentPreview,
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