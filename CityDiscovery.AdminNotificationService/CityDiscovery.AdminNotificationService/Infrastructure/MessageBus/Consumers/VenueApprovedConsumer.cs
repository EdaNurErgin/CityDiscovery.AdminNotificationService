using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Domain.Entities;
using CityDiscovery.AdminNotificationService.Shared.Common.Events.Venue; // Namespace'i projene göre ayarla
using CityDiscovery.VenueService.VenuesService.Shared.Common.Events.Venue;
using MassTransit;
using System.Text.Json;

namespace CityDiscovery.AdminNotificationService.Infrastructure.MessageBus.Consumers
{
    public class VenueApprovedConsumer : IConsumer<VenueApprovedEvent>
    {
        private readonly INotificationRepository _notificationRepository;

        public VenueApprovedConsumer(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task Consume(ConsumeContext<VenueApprovedEvent> context)
        {
            var msg = context.Message;

            // Payload için anonim nesne oluşturup JSON'a çeviriyoruz
            var payloadData = new
            {
                VenueId = msg.VenueId,
                VenueName = msg.VenueName,
                Message = $"'{msg.VenueName}' isimli mekanınız onaylandı." // Frontend'de göstermek için opsiyonel
            };

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = msg.OwnerUserId,
                Type = "VenueApproved", // Frontend bu tipe göre ikon/renk seçecek
                ActorUserId = null, // Sistem tarafından yapıldığı için null
                Payload = JsonSerializer.Serialize(payloadData),
                TargetType = "Venue",
                TargetId = msg.VenueId,
                Route = $"/venue-detail/{msg.VenueId}", // Frontend'deki route yapısı
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(notification);
            // SaveChangesAsync repository içinde çağrılmıyorsa burada çağır:
            // await _notificationRepository.SaveChangesAsync(); 
        }
    }
}