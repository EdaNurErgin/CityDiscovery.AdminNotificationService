using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.Notifications.Queries.GetUnreadCount
{
    public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, int>
    {
        private readonly INotificationRepository _notificationRepository;

        public GetUnreadCountQueryHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<int> Handle(
            GetUnreadCountQuery request,
            CancellationToken cancellationToken)
        {
            return await _notificationRepository
                .GetUnreadCountAsync(request.UserId, cancellationToken);
        }
    }
}
