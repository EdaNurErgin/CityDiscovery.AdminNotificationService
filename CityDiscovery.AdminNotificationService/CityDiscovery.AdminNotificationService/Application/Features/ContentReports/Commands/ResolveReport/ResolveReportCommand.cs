using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Commands.ResolveReport
{
    public class ResolveReportCommand : IRequest
    {
        public Guid ReportId { get; set; }
        public Guid AdminUserId { get; set; }

        // "Resolved" veya "Rejected" (SQL CHECK constraint'ine göre)
        public string NewStatus { get; set; } = default!;
    }
}
