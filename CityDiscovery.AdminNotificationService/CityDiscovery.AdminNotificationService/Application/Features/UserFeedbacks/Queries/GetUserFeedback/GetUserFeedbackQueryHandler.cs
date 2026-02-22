using CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.DTOs;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Application.Interfaces.External;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Queries.GetUserFeedback
{
    public class GetUserFeedbackQueryHandler
        : IRequestHandler<GetUserFeedbackQuery, List<UserFeedbackDto>>
    {
        private readonly IUserFeedbackRepository _userFeedbackRepository;
        private readonly IIdentityServiceClient _identityService;

        public GetUserFeedbackQueryHandler(
            IUserFeedbackRepository userFeedbackRepository,
            IIdentityServiceClient identityService)
        {
            _userFeedbackRepository = userFeedbackRepository;
            _identityService = identityService;
        }

        public async Task<List<UserFeedbackDto>> Handle(
            GetUserFeedbackQuery request,
            CancellationToken cancellationToken)
        {
            var list = await _userFeedbackRepository.GetByUserAsync(request.UserId, cancellationToken);

            // Tek kullanıcı olduğu için tekil çağrı yeterli
            var user = await _identityService.GetUserAsync(request.UserId, cancellationToken);

            return list.Select(f =>
            {
                var dto = UserFeedbackDto.FromEntity(f);
                dto.UserName = user?.UserName;
                return dto;
            }).ToList();
        }
    }
}