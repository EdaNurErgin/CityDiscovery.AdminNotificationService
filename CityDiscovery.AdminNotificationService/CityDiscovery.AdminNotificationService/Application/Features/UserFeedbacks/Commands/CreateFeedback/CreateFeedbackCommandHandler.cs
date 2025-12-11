using CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.DTOs;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Domain.Entities;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Commands.CreateFeedback
{
    public class CreateFeedbackCommandHandler
        : IRequestHandler<CreateFeedbackCommand, UserFeedbackDto>
    {
        private readonly IUserFeedbackRepository _userFeedbackRepository;
        private readonly IAdminAuditLogRepository _adminAuditLogRepository;

        public CreateFeedbackCommandHandler(
            IUserFeedbackRepository userFeedbackRepository,
            IAdminAuditLogRepository adminAuditLogRepository)
        {
            _userFeedbackRepository = userFeedbackRepository;
            _adminAuditLogRepository = adminAuditLogRepository;
        }

        public async Task<UserFeedbackDto> Handle(
            CreateFeedbackCommand request,
            CancellationToken cancellationToken)
        {
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

            // Opsiyonel: admin log
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

            return UserFeedbackDto.FromEntity(feedback);
        }
    }
}
