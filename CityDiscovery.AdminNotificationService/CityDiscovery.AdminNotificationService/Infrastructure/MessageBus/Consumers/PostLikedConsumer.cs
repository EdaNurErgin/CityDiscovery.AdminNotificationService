using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Domain.Entities;
using MassTransit;
// Yukarıda tanımladığımız namespace'i ekliyoruz
using SocialService.Shared.Common.Events.Social;

namespace CityDiscovery.AdminNotificationService.Infrastructure.MessageBus.Consumers
{
    public class PostLikedConsumer : IConsumer<PostLikedEvent>
    {
        private readonly INotificationRepository _notificationRepository;

        public PostLikedConsumer(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task Consume(ConsumeContext<PostLikedEvent> context)
        {
            var msg = context.Message;

            // UYARI: SocialService eventinde PostAuthorUserId yok.
            // Bildirimi kime göndereceğimiz eksik. 
            // Şimdilik test amaçlı 'UserId' (Beğenen kişi) veya Guid.Empty kullanılıyor.
            var targetUserId = Guid.Empty; // TODO: PostId kullanılarak Post sahibini bulmak için servis çağrısı yapılmalı.

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = targetUserId,
                Type = "PostLiked",
                ActorUserId = msg.UserId, // SocialService'den gelen 'UserId' (Beğenen Kişi)
                Payload = $"{{\"postId\":\"{msg.PostId}\"}}", // VenueId eventte yok
                TargetType = "Post",
                TargetId = msg.PostId,
                Route = $"/posts/{msg.PostId}",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            // Hedef kullanıcı bulunamadığı için veritabanına kaydederken hata almamak adına kontrol
            if (targetUserId != Guid.Empty)
            {
                await _notificationRepository.AddAsync(notification);
                await _notificationRepository.SaveChangesAsync();
            }
        }
    }
}