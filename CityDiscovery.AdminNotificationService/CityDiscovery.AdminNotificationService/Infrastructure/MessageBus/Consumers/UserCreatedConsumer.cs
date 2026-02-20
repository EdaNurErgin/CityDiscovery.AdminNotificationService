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

        //public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        //{
        //    // Debug için konsola yazalım
        //    Console.WriteLine($"[Consumer] Mesaj yakalandı! Kullanıcı: {context.Message.UserName}, Rol: {context.Message.Role}");

        //    // Sadece "Owner" rolüyle kayıt olanları adminlere bildir
        //    if (context.Message.Role == "Owner")
        //    {
        //        // 1. Identity Service üzerinden admin listesini çek
        //        var admins = await _identityService.GetUsersByRoleAsync("Admin");

        //        if (admins != null && admins.Any())
        //        {
        //            var notifications = new List<Notification>();

        //            foreach (var admin in admins)
        //            {
        //                notifications.Add(new Notification
        //                {
        //                    Id = Guid.NewGuid(),
        //                    UserId = admin.Id, // Bildirim her bir adminin kendi ID'sine gider
        //                    Type = "NewOwnerRegistered",
        //                    Message = $"Yeni mekan sahibi kayıt oldu: {context.Message.UserName} ({context.Message.Email})",
        //                    IsRead = false,
        //                    CreatedAt = DateTime.UtcNow,
        //                    TargetId = context.Message.UserId, // Kaydolan owner'ın ID'si
        //                    TargetType = "User"
        //                });
        //            }

        //            // 2. Toplu olarak kaydet (Repository içindeki AddRangeAsync SaveChanges yapacak)
        //            await _notificationRepository.AddRangeAsync(notifications);

        //            Console.WriteLine($"[Consumer Success] {notifications.Count} adet admin bildirimi oluşturuldu.");
        //        }
        //        else
        //        {
        //            Console.WriteLine("[Consumer Warning] Sistemde bildirim gönderilecek admin bulunamadı.");
        //        }
        //    }
        //}

        //public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        //{
        //    Console.WriteLine($"[Consumer] Mesaj yakalandı! Kullanıcı: {context.Message.UserName}, Rol: {context.Message.Role}");

        //    if (context.Message.Role == "Owner")
        //    {
        //        var admins = await _identityService.GetUsersByRoleAsync("Admin");

        //        if (admins != null && admins.Any())
        //        {
        //            var notifications = new List<Notification>();

        //            foreach (var admin in admins)
        //            {
        //                // 1. Bildirim nesnesini oluştur
        //                var notification = new Notification
        //                {
        //                    Id = Guid.NewGuid(),
        //                    UserId = admin.Id,
        //                    Type = "NewOwnerRegistered",
        //                    Message = $"Yeni mekan sahibi kayıt oldu: {context.Message.UserName} ({context.Message.Email})",
        //                    IsRead = false,
        //                    CreatedAt = DateTime.UtcNow,
        //                    TargetId = context.Message.UserId,
        //                    TargetType = "User"
        //                };

        //                // 2. Listeye ekle (Veritabanına toplu kaydetmek için)
        //                notifications.Add(notification);

        //                // 3. SIGNALR ILE ANLIK GONDER (Döngü içinde olmalı!)
        //                // Burada 'notification' değişkenine erişebiliriz.
        //                await _hubContext.Clients.User(admin.Id.ToString())
        //                    .SendAsync("ReceiveAdminNotification", new
        //                    {
        //                        Id = notification.Id,
        //                        Type = notification.Type,
        //                        Message = notification.Message,
        //                        CreatedAt = notification.CreatedAt
        //                    });
        //            }

        //            // 4. Tüm bildirimleri veritabanına kaydet
        //            await _notificationRepository.AddRangeAsync(notifications);

        //            Console.WriteLine($"[Consumer Success] {notifications.Count} adet admin bildirimi oluşturuldu ve SignalR ile iletildi.");
        //        }
        //    }
        //}

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
