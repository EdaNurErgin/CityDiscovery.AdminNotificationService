using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.Notifications.Queries.GetUnreadCount
{
    public class GetUnreadCountQuery : IRequest<int>
    {
        public Guid UserId { get; set; }

        public GetUnreadCountQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
