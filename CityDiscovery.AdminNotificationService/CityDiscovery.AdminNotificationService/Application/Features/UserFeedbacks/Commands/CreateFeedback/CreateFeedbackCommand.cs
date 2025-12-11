using CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.DTOs;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Commands.CreateFeedback
{
    public class CreateFeedbackCommand : IRequest<UserFeedbackDto>
    {
        public Guid UserId { get; set; }
        public string Type { get; set; } = default!;      // Feedback, Complaint, Suggestion, Bug
        public string? Subject { get; set; }
        public string Message { get; set; } = default!;
    }
}
