namespace CityDiscovery.AdminNotificationService.Application.Features.UserOwnerNotifications.DTOs
{
    public class CreatePostCommentNotificationRequest
    {
        public Guid PostId { get; set; }
        public Guid CommentId { get; set; }
        public Guid CommenterUserId { get; set; }
    }
}
