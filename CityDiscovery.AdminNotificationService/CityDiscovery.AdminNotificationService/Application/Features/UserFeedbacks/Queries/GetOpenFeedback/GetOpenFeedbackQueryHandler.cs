using CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.DTOs;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Queries.GetOpenFeedback
{
    public class GetOpenFeedbackQueryHandler
        : IRequestHandler<GetOpenFeedbackQuery, List<UserFeedbackDto>>
    {
        private readonly IUserFeedbackRepository _userFeedbackRepository;

        public GetOpenFeedbackQueryHandler(IUserFeedbackRepository userFeedbackRepository)
        {
            _userFeedbackRepository = userFeedbackRepository;
        }

        public async Task<List<UserFeedbackDto>> Handle(
            GetOpenFeedbackQuery request,
            CancellationToken cancellationToken)
        {
            var open = await _userFeedbackRepository
                .GetByStatusAsync("Open", cancellationToken);

            var inProgress = await _userFeedbackRepository
                .GetByStatusAsync("InProgress", cancellationToken);

            return open
                .Concat(inProgress)
                .OrderByDescending(f => f.CreatedAt)
                .Select(UserFeedbackDto.FromEntity)
                .ToList();
        }
    }
}
