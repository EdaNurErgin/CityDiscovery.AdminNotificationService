using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CityDiscovery.AdminNotificationService.API.Hubs
{
    // Sadece yetkili adminlerin bağlanmasını sağlamak için Authorize attribute'u ekleyebilirsin
    [Authorize(Roles = "Admin")]
    public class NotificationHub : Hub
    {
        // İstemciden sunucuya özel bir metot tetiklenmeyecekse (sadece sunucudan istemciye veri itilecekse) 
        // bu sınıfın içi boş kalabilir. SignalR Context'i üzerinden dışarıdan tetikleme yapacağız.

        public override async Task OnConnectedAsync()
        {
            // Bağlanan admini loglayabilir veya özel bir gruba ekleyebilirsin
            await base.OnConnectedAsync();
        }
    }
}