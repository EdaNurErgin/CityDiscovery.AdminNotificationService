using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Commands.UpdateFeedbackStatus
{
    public class UpdateFeedbackStatusCommandHandler
        : IRequestHandler<UpdateFeedbackStatusCommand>
    {
        private readonly IUserFeedbackRepository _userFeedbackRepository;
        private readonly IAdminAuditLogRepository _adminAuditLogRepository;

        public UpdateFeedbackStatusCommandHandler(
            IUserFeedbackRepository userFeedbackRepository,
            IAdminAuditLogRepository adminAuditLogRepository)
        {
            _userFeedbackRepository = userFeedbackRepository;
            _adminAuditLogRepository = adminAuditLogRepository;
        }

        public async Task Handle(
            UpdateFeedbackStatusCommand request,
            CancellationToken cancellationToken)
        {
            var feedback = await _userFeedbackRepository
                .GetByIdAsync(request.FeedbackId, cancellationToken);

            if (feedback == null)
                throw new KeyNotFoundException("Feedback not found.");

            // SQL'deki CHECK constraint'e uy: Open, InProgress, Resolved, Closed
            var allowed = new[] { "Open", "InProgress", "Resolved", "Closed" };
            if (!allowed.Contains(request.NewStatus))
                throw new ArgumentException("Invalid feedback status.", nameof(request.NewStatus));

            feedback.Status = request.NewStatus;

            if (request.NewStatus is "Resolved" or "Closed")
                feedback.ResolvedAt = DateTime.UtcNow;

            await _userFeedbackRepository.UpdateAsync(feedback, cancellationToken);
            await _userFeedbackRepository.SaveChangesAsync(cancellationToken);

            var log = new Domain.Entities.AdminAuditLog
            {
                Id = Guid.NewGuid(),
                AdminUserId = request.AdminUserId,
                Action = "UpdateFeedbackStatus",
                TargetType = "Feedback",
                TargetId = feedback.Id,
                Details = $"Feedback {feedback.Id} status changed to {request.NewStatus}",
                CreatedAt = DateTime.UtcNow
            };

            await _adminAuditLogRepository.AddAsync(log, cancellationToken);
            await _adminAuditLogRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
