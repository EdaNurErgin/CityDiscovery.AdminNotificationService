using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CityDiscovery.AdminNotificationService.API.Hubs
{
    [Authorize]
    public class UserOwnerNotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                // Kullanıcıyı her iki kanala da abone ediyoruz
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Owner_{userId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Owner_{userId}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}