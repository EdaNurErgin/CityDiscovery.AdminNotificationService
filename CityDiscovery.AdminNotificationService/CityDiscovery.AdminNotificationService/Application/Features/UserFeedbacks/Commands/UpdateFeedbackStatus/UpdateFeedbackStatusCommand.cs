using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Commands.UpdateFeedbackStatus
{
    public class UpdateFeedbackStatusCommand : IRequest
    {
        public Guid FeedbackId { get; set; }
        public Guid AdminUserId { get; set; }
        public string NewStatus { get; set; } = default!;   // InProgress, Resolved, Closed
    }
}
