using CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.DTOs;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Queries.GetUserFeedback
{
    public class GetUserFeedbackQuery : IRequest<List<UserFeedbackDto>>
    {
        public Guid UserId { get; set; }

        public GetUserFeedbackQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
