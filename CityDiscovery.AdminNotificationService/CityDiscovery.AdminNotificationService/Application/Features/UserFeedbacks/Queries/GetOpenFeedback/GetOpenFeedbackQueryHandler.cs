using CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.DTOs;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Application.Interfaces.External;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Queries.GetOpenFeedback
{
    public class GetOpenFeedbackQueryHandler
        : IRequestHandler<GetOpenFeedbackQuery, List<UserFeedbackDto>>
    {
        private readonly IUserFeedbackRepository _userFeedbackRepository;
        private readonly IIdentityServiceClient _identityService;

        public GetOpenFeedbackQueryHandler(
            IUserFeedbackRepository userFeedbackRepository,
            IIdentityServiceClient identityService)
        {
            _userFeedbackRepository = userFeedbackRepository;
            _identityService = identityService;
        }

        public async Task<List<UserFeedbackDto>> Handle(
            GetOpenFeedbackQuery request,
            CancellationToken cancellationToken)
        {
            var open = await _userFeedbackRepository.GetByStatusAsync("Open", cancellationToken);
            var inProgress = await _userFeedbackRepository.GetByStatusAsync("InProgress", cancellationToken);

            var all = open.Concat(inProgress).OrderByDescending(f => f.CreatedAt).ToList();

            // Toplu kullanıcı çek
            var userIds = all.Select(f => f.UserId).Distinct().ToList();
            var users = await _identityService.GetUsersAsync(userIds, cancellationToken);
            var userMap = users.ToDictionary(u => u.Id, u => u.UserName);

            return all.Select(f =>
            {
                var dto = UserFeedbackDto.FromEntity(f);
                dto.UserName = userMap.TryGetValue(f.UserId, out var name) ? name : null;
                return dto;
            }).ToList();
        }
    }
}