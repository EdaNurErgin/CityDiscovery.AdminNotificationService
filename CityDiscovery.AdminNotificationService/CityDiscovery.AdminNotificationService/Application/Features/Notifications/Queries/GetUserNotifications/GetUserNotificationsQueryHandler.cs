using CityDiscovery.AdminNotificationService.Application.Common.Models;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.DTOs;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.Notifications.Queries.GetUserNotifications
{
    public class GetUserNotificationsQueryHandler
        : IRequestHandler<GetUserNotificationsQuery, PagedResult<NotificationDto>>
    {
        private readonly INotificationRepository _notificationRepository;

        public GetUserNotificationsQueryHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<PagedResult<NotificationDto>> Handle(
            GetUserNotificationsQuery request,
            CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _notificationRepository
                .GetPagedForUserAsync(
                    request.UserId,
                    request.Page,
                    request.PageSize,
                    request.UnreadOnly,
                    cancellationToken);

            var dtoItems = items
                .Select(NotificationDto.FromEntity)
                .ToList();

            return new PagedResult<NotificationDto>
            {
                Items = dtoItems,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
}
