using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.DTOs;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Application.Interfaces.External;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Queries.GetReportsByUserId
{
    public class GetReportsByUserIdQueryHandler
        : IRequestHandler<GetReportsByUserIdQuery, List<ContentReportDto>>
    {
        private readonly IContentReportRepository _contentReportRepository;
        private readonly IIdentityServiceClient _identityService;

        public GetReportsByUserIdQueryHandler(
            IContentReportRepository contentReportRepository,
            IIdentityServiceClient identityService)
        {
            _contentReportRepository = contentReportRepository;
            _identityService = identityService;
        }

        public async Task<List<ContentReportDto>> Handle(
            GetReportsByUserIdQuery request,
            CancellationToken cancellationToken)
        {
            // Kullanıcının oluşturduğu raporları getir
            var list = await _contentReportRepository.GetByReportingUserIdAsync(request.UserId, cancellationToken);

            // Kullanıcı adını IdentityService'den çek (Tek kullanıcı olduğu için tekil çağrı yeterli)
            var user = await _identityService.GetUserAsync(request.UserId, cancellationToken);

            // Verileri DTO'ya maple
            return list.Select(r =>
            {
                var dto = ContentReportDto.FromEntity(r);
                dto.ReportingUserName = user?.UserName;
                return dto;
            }).ToList();
        }
    }
}