using CityDiscovery.AdminNotificationService.Application.Interfaces.External;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Domain.Entities;
using CityDiscovery.VenueService.VenuesService.Shared.Common.Events.Venue;
using MassTransit;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR; 
using CityDiscovery.AdminNotificationService.API.Hubs; 
namespace CityDiscovery.AdminNotificationService.Infrastructure.MessageBus.Consumers
{
    public class VenueCreatedConsumer : IConsumer<VenueCreatedEvent>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IIdentityServiceClient _identityServiceClient;
        private readonly IHubContext<NotificationHub> _hubContext;

        public VenueCreatedConsumer(INotificationRepository notificationRepository, IIdentityServiceClient identityServiceClient, IHubContext<NotificationHub> hubContext)
        {
            _notificationRepository = notificationRepository;
            _identityServiceClient = identityServiceClient;
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<VenueCreatedEvent> context)
        {
            var msg = context.Message;
            var admins = await _identityServiceClient.GetUsersByRoleAsync("Admin");

            if (admins != null && admins.Any())
            {
                var notifications = new List<Notification>();
                var payloadJson = JsonSerializer.Serialize(new
                {
                    VenueId = msg.VenueId,
                    VenueName = msg.Name,
                    Message = "Yeni mekan onayı bekliyor."
                });

                foreach (var admin in admins)
                {
                    notifications.Add(new Notification
                    {
                        Id = Guid.NewGuid(),
                        UserId = admin.Id,
                        Type = "VenueApprovalNeeded",
                        Message = $"'{msg.Name}' isimli yeni mekan onay bekliyor.",
                        ActorUserId = msg.OwnerUserId, // Mekanı açan kişi Actor oluyor
                        Payload = payloadJson,
                        TargetType = "Venue",
                        TargetId = msg.VenueId,
                        Route = $"/admin/venues/{msg.VenueId}/approve", // Admin paneli linki
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _notificationRepository.AddRangeAsync(notifications);

                // SignalR ile Anlık Olarak Adminlere Gönder
                foreach (var notification in notifications)
                {
                    // İlgili adminin UserId'sine göre sadece ona bildirim gönderiyoruz.
                    // Not: SignalR'ın kullanıcının kimliğini eşleştirebilmesi için JWT token içindeki 
                    // 'NameIdentifier' (veya 'sub') claim'inin admin.Id ile aynı olması gerekir.
                    await _hubContext.Clients.User(notification.UserId.ToString())
                        .SendAsync("ReceiveAdminNotification", new
                        {
                            Id = notification.Id,
                            Type = notification.Type,
                            Message = notification.Message,
                            Route = notification.Route,
                            CreatedAt = notification.CreatedAt
                        });
                }
            }
        }
    }
}