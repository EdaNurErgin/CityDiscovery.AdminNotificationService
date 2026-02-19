using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.DeleteNotification
{
    public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand>
    {
        private readonly INotificationRepository _notificationRepository;

        public DeleteNotificationCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            // 1. Önce bildirim var mı ve bu kullanıcıya mı ait?
            var notification = await _notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);

            if (notification == null)
            {
                throw new KeyNotFoundException($"Notification {request.NotificationId} not found.");
            }

            if (notification.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("Bu bildirimi silme yetkiniz yok.");
            }

            // 2. Silme işlemini yap
            await _notificationRepository.DeleteAsync(request.NotificationId, cancellationToken);
        }
    }
}