//namespace CityDiscovery.SocialService.SocialServiceShared.Common.Events.Social
//{
//    public class CommentAddedEvent
//    {
//        public Guid CommentId { get; set; }
//        public Guid PostId { get; set; }
//        public Guid PostAuthorUserId { get; set; } // Bildirimi alacak kişi
//        public Guid CommenterUserId { get; set; }
//        public string CommentBody { get; set; }
//    }
//}

//using System;

//namespace CityDiscovery.SocialService.SocialServiceShared.Common.Events.Social
//{
//    public class CommentAddedEvent
//    {
//        public Guid CommentId { get; set; }
//        public Guid PostId { get; set; }
//        public Guid PostAuthorUserId { get; set; } // Bildirimi alacak kişi
//        public Guid CommenterUserId { get; set; } // Yorumu yapan kişi
//        public string CommentBody { get; set; }
//        public DateTime CreatedAt { get; set; }

//        public CommentAddedEvent() { }

//        public CommentAddedEvent(Guid commentId, Guid postId, Guid postAuthorUserId, Guid commenterUserId, string commentBody)
//        {
//            CommentId = commentId;
//            PostId = postId;
//            PostAuthorUserId = postAuthorUserId;
//            CommenterUserId = commenterUserId;
//            CommentBody = commentBody;
//            CreatedAt = DateTime.UtcNow;
//        }
//    }
//}

namespace CityDiscovery.SocialService.SocialServiceShared.Common.Events.Social
{
    /// <summary>
    /// SocialService tarafından yayınlanan yorum eklendi eventi.
    /// Namespace SocialService ile birebir aynı olmalı (MassTransit routing için).
    /// </summary>
    public class CommentAddedEvent
    {
        public Guid CommentId { get; set; }
        public Guid PostId { get; set; }

        /// <summary>Post sahibinin ID'si — bu kişiye bildirim gidecek.</summary>
        public Guid PostAuthorUserId { get; set; }

        /// <summary>Yorum yapan kişinin ID'si.</summary>
        public Guid UserId { get; set; }

        public string? AuthorUserName { get; set; }
        public string? AuthorAvatarUrl { get; set; }
        public string Content { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}