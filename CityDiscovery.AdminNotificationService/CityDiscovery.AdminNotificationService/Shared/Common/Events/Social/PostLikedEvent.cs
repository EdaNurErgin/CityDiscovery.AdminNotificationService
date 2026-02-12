using System;

// DİKKAT: SocialService'deki namespace ile birebir aynı olmalı
namespace CityDiscovery.SocialService.SocialServiceShared.Common.Events.Social
{
    public class PostLikedEvent
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; } // SocialService'de beğenen kişi 'UserId' olarak geçiyor
        public DateTime LikedAt { get; set; }
    }
}