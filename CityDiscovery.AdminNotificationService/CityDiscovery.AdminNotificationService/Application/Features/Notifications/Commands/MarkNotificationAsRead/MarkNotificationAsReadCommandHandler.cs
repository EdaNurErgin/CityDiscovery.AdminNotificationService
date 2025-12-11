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
            var notification = await _notificationRepository
                .GetByIdAsync(request.NotificationId, cancellationToken);

            if (notification == null)
                return;

            // Başka birinin bildirimini okumasın
            if (notification.UserId != request.UserId)
                return;

            await _notificationRepository.MarkAsReadAsync(request.NotificationId, cancellationToken);
            await _notificationRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
