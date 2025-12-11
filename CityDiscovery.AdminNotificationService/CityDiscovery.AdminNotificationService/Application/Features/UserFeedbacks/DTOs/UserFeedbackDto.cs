using CityDiscovery.AdminNotificationService.Domain.Entities;


namespace CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.DTOs
{
    public class UserFeedbackDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; } = default!;     // Feedback, Complaint, Suggestion, Bug
        public string? Subject { get; set; }
        public string Message { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public static UserFeedbackDto FromEntity(UserFeedback entity)
        {
            return new UserFeedbackDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Type = entity.Type,
                Subject = entity.Subject,
                Message = entity.Message,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                ResolvedAt = entity.ResolvedAt
            };
        }
    }
}