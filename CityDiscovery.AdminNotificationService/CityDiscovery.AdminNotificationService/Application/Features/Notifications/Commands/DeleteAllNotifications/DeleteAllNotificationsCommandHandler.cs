using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.DeleteAllNotifications
{
    public class DeleteAllNotificationsCommandHandler : IRequestHandler<DeleteAllNotificationsCommand>
    {
        private readonly INotificationRepository _notificationRepository;

        public DeleteAllNotificationsCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task Handle(DeleteAllNotificationsCommand request, CancellationToken cancellationToken)
        {
            // Kullanıcıya ait her şeyi sil
            await _notificationRepository.DeleteAllAsync(request.UserId, cancellationToken);
        }
    }
}