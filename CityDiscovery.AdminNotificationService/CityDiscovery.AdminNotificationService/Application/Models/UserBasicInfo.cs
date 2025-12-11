namespace CityDiscovery.AdminNotificationService.Application.Models
{
    public class UserBasicInfo
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? AvatarUrl { get; set; }
    }
}
