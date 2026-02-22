using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.MarkAsRead
{
    public class MarkNotificationAsReadCommandHandler
        : IRequestHandler<MarkNotificationAsReadCommand>
    {
        private readonly INotificationRepository _notificationRepository;

        public MarkNotificationAsReadCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task Handle(
            MarkNotificationAsReadCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Bildirimi kontrol için çek
            var notification = await _notificationRepository
                .GetByIdAsync(request.NotificationId, cancellationToken);

            // Sessizce dönme, hata fırlat!
            if (notification == null)
            {
                throw new KeyNotFoundException($"Notification ID {request.NotificationId} not found.");
            }

            // Sahiplik kontrolü: Token'daki User ile Bildirimdeki User aynı mı?
            if (notification.UserId != request.UserId)
            {
                // Burası sessizce return ederse, API 204 döner ama veri değişmez!
                // Hata fırlat ki sorunu anlayalım.
                throw new UnauthorizedAccessException("Bu bildirim size ait değil!");
            }

            // 2. Repository metodunu çağır (İçinde zaten SaveChanges var)
            await _notificationRepository.MarkAsReadAsync(request.NotificationId, cancellationToken);

            // BURADAKİ SaveChangesAsync'İ SİL. Repository zaten yapıyor.
        }
    }
}