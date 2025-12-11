namespace CityDiscovery.AdminNotificationService.Shared.Common.Events.Social
{
    public class CommentAddedEvent
    {
        public Guid CommentId { get; set; }
        public Guid PostId { get; set; }
        public Guid PostAuthorUserId { get; set; } // Bildirimi alacak kişi
        public Guid CommenterUserId { get; set; }
        public string CommentBody { get; set; }
    }
}