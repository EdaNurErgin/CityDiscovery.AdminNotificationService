using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Application.Interfaces.External; // IIdentityServiceClient için
using CityDiscovery.AdminNotificationService.Domain.Entities;
using IdentityService.Shared.MessageBus.Identity;
using MassTransit;
using Microsoft.AspNetCore.SignalR; 
using CityDiscovery.AdminNotificationService.API.Hubs;

namespace CityDiscovery.AdminNotificationService.Infrastructure.MessageBus.Consumers
{
    public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IIdentityServiceClient _identityService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public UserCreatedConsumer(
            INotificationRepository notificationRepository,
            IIdentityServiceClient identityService, IHubContext<NotificationHub> hubContext)
        {
            _notificationRepository = notificationRepository;
            _identityService = identityService;
            _hubContext = hubContext;
        }


        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var role = context.Message.Role;

            // Hem Owner hem de Admin kayıtlarını takip edelim
            if (role == "Owner" || role == "Admin")
            {
                var admins = await _identityService.GetUsersByRoleAsync("Admin");
                if (admins != null && admins.Any())
                {
                    var notifications = new List<Notification>();

                    // Bildirim tipini role göre belirleyelim
                    string notificationType = (role == "Admin") ? "SecurityAlert_NewAdmin" : "NewOwnerRegistered";

                    foreach (var admin in admins)
                    {
                        var notification = new Notification
                        {
                            Id = Guid.NewGuid(),
                            UserId = admin.Id,
                            Type = notificationType, // Burada dinamik tip kullanıyoruz
                            Message = $"{role} rolüyle yeni kayıt: {context.Message.UserName}",
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow,
                            TargetId = context.Message.UserId,
                            TargetType = "User"
                        };
                        notifications.Add(notification);

                        // SIGNALR: Anlık olarak admin ekranına gönder
                        await _hubContext.Clients.User(admin.Id.ToString())
                            .SendAsync("ReceiveAdminNotification", new
                            {
                                Id = notification.Id,
                                Type = notification.Type,
                                Message = notification.Message,
                                CreatedAt = notification.CreatedAt
                            });
                    }
                    await _notificationRepository.AddRangeAsync(notifications);
                }
            }
        }
    }
}
