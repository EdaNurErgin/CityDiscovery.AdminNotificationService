using System;

// Namespace SocialService ile birebir aynı olmalı — MassTransit routing için
namespace CityDiscovery.SocialService.SocialServiceShared.Common.Events.Social
{
    public class PostLikedEvent
    {
        public Guid PostId { get; set; }
        public Guid PostAuthorUserId { get; set; } // Post sahibi — bildirim bu kişiye gidecek
        public Guid UserId { get; set; }           // Beğenen kişi
        public DateTime LikedAt { get; set; }
    }
}