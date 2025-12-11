using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Commands.ResolveReport
{
    public class ResolveReportCommandHandler : IRequestHandler<ResolveReportCommand>
    {
        private readonly IContentReportRepository _contentReportRepository;
        private readonly IAdminAuditLogRepository _auditLogRepository;

        public ResolveReportCommandHandler(
            IContentReportRepository contentReportRepository,
            IAdminAuditLogRepository adminAuditLogRepository)
        {
            _contentReportRepository = contentReportRepository;
            _auditLogRepository = adminAuditLogRepository;
        }

        public async Task Handle(
            ResolveReportCommand request,
            CancellationToken cancellationToken)
        {
            var report = await _contentReportRepository
                .GetByIdAsync(request.ReportId, cancellationToken);

            if (report == null)
                throw new KeyNotFoundException("Content report not found.");

            if (report.Status != "Open")
                return; // zaten çözümlenmiş, istersen exception da atabilirsin

            if (request.NewStatus != "Resolved" && request.NewStatus != "Rejected")
                throw new ArgumentException("NewStatus must be 'Resolved' or 'Rejected'.");

            report.Status = request.NewStatus;
            report.ResolvedAt = DateTime.UtcNow;
            report.ResolvedByUserId = request.AdminUserId;

            await _contentReportRepository.UpdateAsync(report, cancellationToken);
            await _contentReportRepository.SaveChangesAsync(cancellationToken);

            // Admin log
            var log = new Domain.Entities.AdminAuditLog
            {
                Id = Guid.NewGuid(),
                AdminUserId = request.AdminUserId,
                Action = request.NewStatus == "Resolved" ? "ResolveReport" : "RejectReport",
                TargetType = report.ReportedType,
                TargetId = report.ReportedId,
                Details = $"Report {report.Id} set to {request.NewStatus}",
                CreatedAt = DateTime.UtcNow
            };

            await _auditLogRepository.AddAsync(log, cancellationToken);
            await _auditLogRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
