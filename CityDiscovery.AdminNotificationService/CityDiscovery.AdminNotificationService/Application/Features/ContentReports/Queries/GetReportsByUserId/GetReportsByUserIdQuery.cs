using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.DTOs;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Queries.GetReportsByUserId
{
    public class GetReportsByUserIdQuery : IRequest<List<ContentReportDto>>
    {
        public Guid UserId { get; set; }

        public GetReportsByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}