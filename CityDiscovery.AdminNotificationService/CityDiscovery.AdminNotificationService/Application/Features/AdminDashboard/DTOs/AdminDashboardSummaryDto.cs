namespace CityDiscovery.AdminNotificationService.Application.Features.AdminDashboard.DTOs
{
    public class AdminDashboardSummaryDto
    {
        public int TotalFeedback { get; set; }
        public int OpenReports { get; set; }
        public int UnreadNotifications { get; set; }
    }
}
