using CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.DTOs;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Queries.GetOpenFeedback
{
    public class GetOpenFeedbackQuery : IRequest<List<UserFeedbackDto>>
    {
        // İleride filtre vs. eklersen buraya property ekleyebilirsin
    }
}
