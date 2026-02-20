using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.DTOs;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Application.Interfaces.External; // IIdentityServiceClient için
using CityDiscovery.AdminNotificationService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.SignalR; // SignalR için eklendi
using CityDiscovery.AdminNotificationService.API.Hubs; // Hub için eklendi

namespace CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Commands.CreateReport
{
    public class CreateReportCommandHandler
        : IRequestHandler<CreateReportCommand, ContentReportDto>
    {
        private readonly IContentReportRepository _contentReportRepository;
        private readonly IAdminAuditLogRepository _auditLogRepository;

        // Yeni Eklenen Bağımlılıklar
        private readonly INotificationRepository _notificationRepository;
        private readonly IIdentityServiceClient _identityService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public CreateReportCommandHandler(
            IContentReportRepository contentReportRepository,
            IAdminAuditLogRepository auditLogRepository,
            INotificationRepository notificationRepository,
            IIdentityServiceClient identityService,
            IHubContext<NotificationHub> hubContext)
        {
            _contentReportRepository = contentReportRepository;
            _auditLogRepository = auditLogRepository;
            _notificationRepository = notificationRepository;
            _identityService = identityService;
            _hubContext = hubContext;
        }

        public async Task<ContentReportDto> Handle(
            CreateReportCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Şikayeti Kaydet (Mevcut Kodun)
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

            // 2. Opsiyonel: admin log 
            var log = new AdminAuditLog
            {
                Id = Guid.NewGuid(),
                AdminUserId = Guid.Empty, // sistem
                Action = "CreateReport",
                TargetType = report.ReportedType,
                TargetId = report.ReportedId,
                Details = $"User {report.ReportingUserId} reported {report.ReportedType} {report.ReportedId}",
                CreatedAt = DateTime.UtcNow
            };

            await _auditLogRepository.AddAsync(log, cancellationToken);
            await _auditLogRepository.SaveChangesAsync(cancellationToken);

      
            var admins = await _identityService.GetUsersByRoleAsync("Admin");

            if (admins != null && admins.Any())
            {
                var notifications = new List<Notification>();

                // Dinamik Tip: Şikayet edilen türe göre (Örn: NewReport_Post, NewReport_Comment, NewReport_Venue)
                string notificationType = $"NewReport_{report.ReportedType}";

                foreach (var admin in admins)
                {
                    var notification = new Notification
                    {
                        Id = Guid.NewGuid(),
                        UserId = admin.Id,
                        Type = notificationType,
                        Message = $"Yeni bir '{report.ReportedType}' şikayeti geldi. Sebep: {report.Reason}",
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow,
                        TargetId = report.Id, // Adminin bu şikayete direkt ulaşması için
                        TargetType = "ContentReport",
                        Route = $"/admin/reports/{report.Id}" // Admin panelindeki yönlendirme rotası
                    };
                    notifications.Add(notification);

                    // SIGNALR: Anlık olarak admin ekranına fırlat
                    await _hubContext.Clients.User(admin.Id.ToString())
                        .SendAsync("ReceiveAdminNotification", new
                        {
                            Id = notification.Id,
                            Type = notification.Type,
                            Message = notification.Message,
                            Route = notification.Route,
                            CreatedAt = notification.CreatedAt
                        }, cancellationToken);
                }

                // Tüm bildirimleri veritabanına kaydet
                await _notificationRepository.AddRangeAsync(notifications);
            }

            return ContentReportDto.FromEntity(report);
        }
    }
}