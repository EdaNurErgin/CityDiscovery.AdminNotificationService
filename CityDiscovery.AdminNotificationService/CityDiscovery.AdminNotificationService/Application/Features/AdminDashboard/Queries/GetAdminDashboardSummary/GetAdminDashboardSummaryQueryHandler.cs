using CityDiscovery.AdminNotificationService.Application.Features.AdminDashboard.DTOs;
using CityDiscovery.AdminNotificationService.Application.Features.AdminDashboard.Queries.GetAdminDashboardSummary;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.AdminDashboard.Queries.GetAdminDashboardSummary
{
    public class GetAdminDashboardSummaryQueryHandler
        : IRequestHandler<GetAdminDashboardSummaryQuery, AdminDashboardSummaryDto>
    {
        private readonly IUserFeedbackRepository _userFeedbackRepository;
        private readonly IContentReportRepository _contentReportRepository;
        private readonly INotificationRepository _notificationRepository;

        public GetAdminDashboardSummaryQueryHandler(
            IUserFeedbackRepository userFeedbackRepository,
            IContentReportRepository contentReportRepository,
            INotificationRepository notificationRepository)
        {
            _userFeedbackRepository = userFeedbackRepository;
            _contentReportRepository = contentReportRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<AdminDashboardSummaryDto> Handle(
            GetAdminDashboardSummaryQuery request,
            CancellationToken cancellationToken)
        {
            // 🔹 Toplam feedback sayısı
            var totalFeedback = await _userFeedbackRepository
                .GetTotalCountAsync(cancellationToken);

            // 🔹 Açık (Open) report sayısı
            var openReports = await _contentReportRepository
                .GetOpenCountAsync(cancellationToken);

            // 🔹 Bu admin için okunmamış bildirim sayısı
            var unreadNotifications = await _notificationRepository
                .GetUnreadCountAsync(request.AdminUserId, cancellationToken);

            return new AdminDashboardSummaryDto
            {
                TotalFeedback = totalFeedback,
                OpenReports = openReports,
                UnreadNotifications = unreadNotifications
            };
        }
    }
}
