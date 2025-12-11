using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.DTOs;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Domain.Entities;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Commands.CreateReport
{
    public class CreateReportCommandHandler
        : IRequestHandler<CreateReportCommand, ContentReportDto>
    {
        private readonly IContentReportRepository _contentReportRepository;
        private readonly IAdminAuditLogRepository _auditLogRepository;

        public CreateReportCommandHandler(
            IContentReportRepository contentReportRepository,
            IAdminAuditLogRepository auditLogRepository)
        {
            _contentReportRepository = contentReportRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<ContentReportDto> Handle(
            CreateReportCommand request,
            CancellationToken cancellationToken)
        {
            var report = new ContentReport
            {
                Id = Guid.NewGuid(),
                ReportedType = request.ReportedType,
                ReportedId = request.ReportedId,
                ReportingUserId = request.ReportingUserId,
                Reason = request.Reason,
                Status = "Open",
                CreatedAt = DateTime.UtcNow
            };

            await _contentReportRepository.AddAsync(report, cancellationToken);
            await _contentReportRepository.SaveChangesAsync(cancellationToken);

            // Opsiyonel: admin log (şikayet oluşturuldu)
            var log = new AdminAuditLog
            {
                Id = Guid.NewGuid(),
                AdminUserId = Guid.Empty, // buraya istersen system / 0 yazabilirsin
                Action = "CreateReport",
                TargetType = report.ReportedType,
                TargetId = report.ReportedId,
                Details = $"User {report.ReportingUserId} reported {report.ReportedType} {report.ReportedId}",
                CreatedAt = DateTime.UtcNow
            };

            await _auditLogRepository.AddAsync(log, cancellationToken);
            await _auditLogRepository.SaveChangesAsync(cancellationToken);

            return ContentReportDto.FromEntity(report);
        }
    }
}
