using CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.DTOs;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Application.Interfaces.External; 
using CityDiscovery.AdminNotificationService.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.SignalR; 
using CityDiscovery.AdminNotificationService.API.Hubs; 

namespace CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Commands.CreateFeedback
{
    public class CreateFeedbackCommandHandler
        : IRequestHandler<CreateFeedbackCommand, UserFeedbackDto>
    {
        private readonly IUserFeedbackRepository _userFeedbackRepository;
        private readonly IAdminAuditLogRepository _adminAuditLogRepository;

        // Yeni Eklenen Bağımlılıklar
        private readonly INotificationRepository _notificationRepository;
        private readonly IIdentityServiceClient _identityService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public CreateFeedbackCommandHandler(
            IUserFeedbackRepository userFeedbackRepository,
            IAdminAuditLogRepository adminAuditLogRepository,
            INotificationRepository notificationRepository,
            IIdentityServiceClient identityService,
            IHubContext<NotificationHub> hubContext)
        {
            _userFeedbackRepository = userFeedbackRepository;
            _adminAuditLogRepository = adminAuditLogRepository;
            _notificationRepository = notificationRepository;
            _identityService = identityService;
            _hubContext = hubContext;
        }

        public async Task<UserFeedbackDto> Handle(
            CreateFeedbackCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Geri Bildirimi Kaydet (Senin Kodun)
            var feedback = new UserFeedback
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Type = request.Type,
                Subject = request.Subject,
                Message = request.Message,
                Status = "Open",
                CreatedAt = DateTime.UtcNow
            };

            await _userFeedbackRepository.AddAsync(feedback, cancellationToken);
            await _userFeedbackRepository.SaveChangesAsync(cancellationToken);

            //  Admin Log 
            var log = new AdminAuditLog
            {
                Id = Guid.NewGuid(),
                AdminUserId = Guid.Empty, // sistem
                Action = "CreateFeedback",
                TargetType = "Feedback",
                TargetId = feedback.Id,
                Details = $"User {feedback.UserId} sent {feedback.Type} feedback.",
                CreatedAt = DateTime.UtcNow
            };

            await _adminAuditLogRepository.AddAsync(log, cancellationToken);
            await _adminAuditLogRepository.SaveChangesAsync(cancellationToken);

        
            var admins = await _identityService.GetUsersByRoleAsync("Admin");

            if (admins != null && admins.Any())
            {
                var notifications = new List<Notification>();

                // Türüne göre dinamik bildirim tipi (Örn: NewFeedback_Bug, NewFeedback_Suggestion)
                string notificationType = $"NewFeedback_{feedback.Type}";

                foreach (var admin in admins)
                {
                    var notification = new Notification
                    {
                        Id = Guid.NewGuid(),
                        UserId = admin.Id,
                        Type = notificationType,
                        Message = $"Yeni bir '{feedback.Type}' bildirimi geldi: {feedback.Subject}",
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow,
                        TargetId = feedback.Id,
                        TargetType = "UserFeedback",
                        Route = $"/admin/feedbacks/{feedback.Id}"
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

                // Bildirimleri veritabanına kaydet
                await _notificationRepository.AddRangeAsync(notifications);

           
            }

            return UserFeedbackDto.FromEntity(feedback);
        }
    }
}