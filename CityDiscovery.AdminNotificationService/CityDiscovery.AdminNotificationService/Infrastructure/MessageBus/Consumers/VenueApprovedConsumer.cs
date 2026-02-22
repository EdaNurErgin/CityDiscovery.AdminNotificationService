using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Domain.Entities;
using CityDiscovery.VenueService.VenuesService.Shared.Common.Events.Venue;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using CityDiscovery.AdminNotificationService.API.Hubs;
using System.Text.Json;

namespace CityDiscovery.AdminNotificationService.Infrastructure.MessageBus.Consumers
{
    public class VenueApprovedConsumer : IConsumer<VenueApprovedEvent>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<UserNotificationHub> _userHubContext;
        private readonly ILogger<VenueApprovedConsumer> _logger;

        public VenueApprovedConsumer(
            INotificationRepository notificationRepository,
            IHubContext<UserNotificationHub> userHubContext,
            ILogger<VenueApprovedConsumer> logger)
        {
            _notificationRepository = notificationRepository;
            _userHubContext = userHubContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<VenueApprovedEvent> context)
        {
            var msg = context.Message;

            var payloadJson = JsonSerializer.Serialize(new
            {
                VenueId = msg.VenueId,
                VenueName = msg.VenueName
            });

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = msg.OwnerUserId,
                Type = "VenueApproved",
                Message = $"'{msg.VenueName}' isimli mekanınız onaylandı.",
                ActorUserId = null, // Sistem tarafından
                Payload = payloadJson,
                TargetType = "Venue",
                TargetId = msg.VenueId,
                Route = $"/venue-detail/{msg.VenueId}",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            // Veritabanına kaydet
            await _notificationRepository.AddAsync(notification);
            await _notificationRepository.SaveChangesAsync(); // ← DÜZELTİLDİ

            _logger.LogInformation(
                "Mekan onay bildirimi kaydedildi. Owner: {OwnerId}, VenueId: {VenueId}",
                msg.OwnerUserId, msg.VenueId);

            // SignalR ile mekan sahibine anlık bildirim
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
                        VenueId = msg.VenueId,
                        VenueName = msg.VenueName,
                        CreatedAt = notification.CreatedAt
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SignalR gönderimi başarısız. UserId: {OwnerId}", msg.OwnerUserId);
            }
        }
    }
}