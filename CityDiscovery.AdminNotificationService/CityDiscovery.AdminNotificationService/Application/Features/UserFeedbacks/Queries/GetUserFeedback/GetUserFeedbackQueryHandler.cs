using CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.DTOs;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Queries.GetUserFeedback
{
    public class GetUserFeedbackQueryHandler
        : IRequestHandler<GetUserFeedbackQuery, List<UserFeedbackDto>>
    {
        private readonly IUserFeedbackRepository _userFeedbackRepository;

        public GetUserFeedbackQueryHandler(IUserFeedbackRepository userFeedbackRepository)
        {
            _userFeedbackRepository = userFeedbackRepository;
        }

        public async Task<List<UserFeedbackDto>> Handle(
            GetUserFeedbackQuery request,
            CancellationToken cancellationToken)
        {
            var list = await _userFeedbackRepository
                .GetByUserAsync(request.UserId, cancellationToken);

            return list.Select(UserFeedbackDto.FromEntity).ToList();
        }
    }
}
