using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CityDiscovery.AdminNotificationService.API.Hubs
{
    /// <summary>
    /// Normal kullanıcılar için bildirim hub'ı.
    /// Endpoint: /hubs/user-notifications
    /// Kullanım: Yorum, beğeni, mekan onayı gibi user-facing bildirimler
    /// </summary>
    [Authorize] // Rol kısıtlaması yok — tüm authenticate olmuş kullanıcılar bağlanabilir
    public class UserNotificationHub : Hub
    {
        private readonly ILogger<UserNotificationHub> _logger;

        public UserNotificationHub(ILogger<UserNotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            _logger.LogInformation("[UserNotificationHub] Kullanıcı bağlandı. UserId: {UserId}, ConnectionId: {ConnectionId}",
                userId, Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            _logger.LogInformation("[UserNotificationHub] Kullanıcı ayrıldı. UserId: {UserId}, ConnectionId: {ConnectionId}",
                userId, Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}