//using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
//using CityDiscovery.AdminNotificationService.Domain.Entities;
//using CityDiscovery.ReviewService.ReviewService.Shared.Events.Venue;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Application.Interfaces.External;
using CityDiscovery.AdminNotificationService.Domain.Entities;
using CityDiscovery.ReviewService.ReviewService.Shared.Events.Venue;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using CityDiscovery.AdminNotificationService.API.Hubs;
using System.Text.Json;

namespace CityDiscovery.AdminNotificationService.Infrastructure.MessageBus.Consumers
{
    public class VenueFavoritedConsumer : IConsumer<VenueFavoritedEvent>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IIdentityServiceClient _identityServiceClient;
        private readonly IHubContext<UserNotificationHub> _userHubContext;
        private readonly ILogger<VenueFavoritedConsumer> _logger;

        public VenueFavoritedConsumer(
            INotificationRepository notificationRepository,
            IIdentityServiceClient identityServiceClient,
            IHubContext<UserNotificationHub> userHubContext,
            ILogger<VenueFavoritedConsumer> logger)
        {
            _notificationRepository = notificationRepository;
            _identityServiceClient = identityServiceClient;
            _userHubContext = userHubContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<VenueFavoritedEvent> context)
        {
            var msg = context.Message;

            if (msg.OwnerUserId == Guid.Empty)
            {
                _logger.LogWarning("OwnerUserId boş geldi. VenueId: {VenueId}", msg.VenueId);
                return;
            }

            if (msg.OwnerUserId == msg.UserId)
                return;

            // Favorileyen kişinin adını çek
            var actor = await _identityServiceClient.GetUserAsync(msg.UserId);
            var actorName = actor?.UserName ?? "Bir kullanıcı";

            var payloadJson = JsonSerializer.Serialize(new
            {
                VenueId = msg.VenueId,
                FavoritedByUserId = msg.UserId,
                FavoritedByUserName = actorName,
                FavoritedByAvatarUrl = actor?.AvatarUrl,
                FavoritedAt = msg.FavoritedAt
            });

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = msg.OwnerUserId,
                Type = "VenueFavorited",
                Message = $"{actorName} mekanınızı favorilerine ekledi.",
                ActorUserId = msg.UserId,
                Payload = payloadJson,
                TargetType = "Venue",
                TargetId = msg.VenueId,
                Route = $"/venues/{msg.VenueId}",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
            await _notificationRepository.SaveChangesAsync();

            try
            {
                await _userHubContext.Clients
                    .User(msg.OwnerUserId.ToString())
                    .SendAsync("ReceiveNotification", new
                    {
                        Id = notification.Id,
                        Type = notification.Type,
                        Message = notification.Message,
                        Route = notification.Route,
                        ActorUserId = msg.UserId,
                        ActorUserName = actorName,
                        ActorAvatarUrl = actor?.AvatarUrl,
                        VenueId = msg.VenueId,
                        CreatedAt = notification.CreatedAt
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR gönderimi başarısız. OwnerId: {OwnerId}", msg.OwnerUserId);
            }
        }
    }
}