using CityDiscovery.AdminNotificationService.Application.Features.AdminDashboard.DTOs;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.AdminDashboard.Queries.GetAdminDashboardSummary
{
    public class GetAdminDashboardSummaryQuery : IRequest<AdminDashboardSummaryDto>
    {
        // Admin panelde hangi admin için unread notifications bakılacak
        public Guid AdminUserId { get; set; }

        public GetAdminDashboardSummaryQuery(Guid adminUserId)
        {
            AdminUserId = adminUserId;
        }
    }
}
