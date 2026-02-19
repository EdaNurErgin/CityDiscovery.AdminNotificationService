using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Application.Interfaces.External; // IIdentityServiceClient için
using CityDiscovery.AdminNotificationService.Domain.Entities;
using IdentityService.Shared.MessageBus.Identity;
using MassTransit;

namespace CityDiscovery.AdminNotificationService.Infrastructure.MessageBus.Consumers
{
    public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IIdentityServiceClient _identityService;

        public UserCreatedConsumer(
            INotificationRepository notificationRepository,
            IIdentityServiceClient identityService)
        {
            _notificationRepository = notificationRepository;
            _identityService = identityService;
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            // Debug için konsola yazalım
            Console.WriteLine($"[Consumer] Mesaj yakalandı! Kullanıcı: {context.Message.UserName}, Rol: {context.Message.Role}");

            // Sadece "Owner" rolüyle kayıt olanları adminlere bildir
            if (context.Message.Role == "Owner")
            {
                // 1. Identity Service üzerinden admin listesini çek
                var admins = await _identityService.GetUsersByRoleAsync("Admin");

                if (admins != null && admins.Any())
                {
                    var notifications = new List<Notification>();

                    foreach (var admin in admins)
                    {
                        notifications.Add(new Notification
                        {
                            Id = Guid.NewGuid(),
                            UserId = admin.Id, // Bildirim her bir adminin kendi ID'sine gider
                            Type = "NewOwnerRegistered",
                            Message = $"Yeni mekan sahibi kayıt oldu: {context.Message.UserName} ({context.Message.Email})",
                            IsRead = false,
                            CreatedAt = DateTime.UtcNow,
                            TargetId = context.Message.UserId, // Kaydolan owner'ın ID'si
                            TargetType = "User"
                        });
                    }

                    // 2. Toplu olarak kaydet (Repository içindeki AddRangeAsync SaveChanges yapacak)
                    await _notificationRepository.AddRangeAsync(notifications);

                    Console.WriteLine($"[Consumer Success] {notifications.Count} adet admin bildirimi oluşturuldu.");
                }
                else
                {
                    Console.WriteLine("[Consumer Warning] Sistemde bildirim gönderilecek admin bulunamadı.");
                }
            }
        }
    }
}
