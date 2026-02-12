using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Shared.Common.Events.AdminNotification;
using MassTransit;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Commands.ResolveReport
{
    public class ResolveReportCommandHandler : IRequestHandler<ResolveReportCommand>
    {
        private readonly IContentReportRepository _contentReportRepository;
        private readonly IAdminAuditLogRepository _auditLogRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public ResolveReportCommandHandler(
            IContentReportRepository contentReportRepository,
            IAdminAuditLogRepository adminAuditLogRepository,
            IPublishEndpoint publishEndpoint)
        {
            _contentReportRepository = contentReportRepository;
            _auditLogRepository = adminAuditLogRepository;
            _publishEndpoint = publishEndpoint;
        }

        //public async Task Handle(
        //    ResolveReportCommand request,
        //    CancellationToken cancellationToken)
        //{
        //    var report = await _contentReportRepository
        //        .GetByIdAsync(request.ReportId, cancellationToken);

        //    if (report == null)
        //        throw new KeyNotFoundException("Content report not found.");

        //    if (report.Status != "Open")
        //        return; // zaten çözümlenmiş, istersen exception da atabilirsin

        //    if (request.NewStatus != "Resolved" && request.NewStatus != "Rejected")
        //        throw new ArgumentException("NewStatus must be 'Resolved' or 'Rejected'.");

        //    report.Status = request.NewStatus;
        //    report.ResolvedAt = DateTime.UtcNow;
        //    report.ResolvedByUserId = request.AdminUserId;

        //    await _contentReportRepository.UpdateAsync(report, cancellationToken);
        //    await _contentReportRepository.SaveChangesAsync(cancellationToken);

        //    // Admin log
        //    var log = new Domain.Entities.AdminAuditLog
        //    {
        //        Id = Guid.NewGuid(),
        //        AdminUserId = request.AdminUserId,
        //        Action = request.NewStatus == "Resolved" ? "ResolveReport" : "RejectReport",
        //        TargetType = report.ReportedType,
        //        TargetId = report.ReportedId,
        //        Details = $"Report {report.Id} set to {request.NewStatus}",
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    await _auditLogRepository.AddAsync(log, cancellationToken);
        //    await _auditLogRepository.SaveChangesAsync(cancellationToken);
        //}

        public async Task Handle(
            ResolveReportCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Raporu Bul
            var report = await _contentReportRepository
                .GetByIdAsync(request.ReportId, cancellationToken);

            if (report == null)
                throw new KeyNotFoundException("Content report not found.");

            if (report.Status != "Open")
                return;

            if (request.NewStatus != "Resolved" && request.NewStatus != "Rejected")
                throw new ArgumentException("NewStatus must be 'Resolved' or 'Rejected'.");

            // 2. Rapor Durumunu Güncelle (Veritabanı İşlemi)
            report.Status = request.NewStatus;
            report.ResolvedAt = DateTime.UtcNow;
            report.ResolvedByUserId = request.AdminUserId;

            await _contentReportRepository.UpdateAsync(report, cancellationToken);
            await _contentReportRepository.SaveChangesAsync(cancellationToken);

            // 3. Admin Logu Oluştur
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

            // 🚀 4. EĞER DURUM 'RESOLVED' İSE SİLME EMRİ GÖNDER (YENİ KISIM)
            // Sadece şikayet haklı bulunduğunda (Resolved) çalışır.
            if (request.NewStatus == "Resolved")
            {
                await _publishEndpoint.Publish(new ContentRemovedEvent
                {
                    ContentId = report.ReportedId,      // Silinecek Yorum/Mekan ID'si
                    ContentType = report.ReportedType,  // "Comment", "Venue", "Post" vb.
                    Reason = "Admin tarafından onaylanan şikayet üzerine kaldırıldı."
                }, cancellationToken);
            }
        }
    }
}
