using CityDiscovery.AdminNotificationService.Application.Common.Models;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.DTOs;
using CityDiscovery.AdminNotificationService.Application.Interfaces.ExternalServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityDiscovery.AdminNotificationService.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user-owner-notifications")]
    public class UserOwnerNotificationsController : ControllerBase
    {
        private readonly IUserOwnerNotificationService _notificationService;

        public UserOwnerNotificationsController(IUserOwnerNotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Kullanıcıya veya Mekan Sahibine ait bildirimleri sayfalı (pagination) olarak listeler.
        /// </summary>
        /// <param name="recipientId">Bildirimleri listelenecek kullanıcının veya mekan sahibinin ID'si (Guid formatında).</param>
        /// <param name="category">Filtreleme türü: Sadece kullanıcı bildirimleri için "User", sadece mekan bildirimleri için "Owner" yazın. Boş bırakılırsa tümü gelir.</param>
        /// <param name="unreadOnly">True gönderilirse sadece okunmamış (yeni) bildirimler listelenir.</param>
        /// <param name="page">Sayfa numarası (Örn: 1)</param>
        /// <param name="pageSize">Bir sayfada getirilecek bildirim sayısı (Örn: 20)</param>
        /// <returns>Sayfalanmış bildirim listesi döner.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<NotificationDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<NotificationDto>>> GetNotifications(
            [FromQuery] Guid recipientId,
            [FromQuery] string category = "",
            [FromQuery] bool unreadOnly = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _notificationService.GetNotificationsAsync(category, recipientId, unreadOnly, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Kullanıcının veya Mekan Sahibinin okunmamış (yeni) bildirimlerinin toplam sayısını getirir.
        /// </summary>
        /// <param name="recipientId">Kullanıcının veya mekan sahibinin ID'si.</param>
        /// <param name="category">Sadece belirli bir kategorideki okunmamışları saymak için "User" veya "Owner" yazabilirsiniz. Boş bırakılabilir.</param>
        /// <returns>Okunmamış bildirimlerin toplam sayısı (int) döner.</returns>
        [HttpGet("unread-count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetUnreadCount(
            [FromQuery] Guid recipientId,
            [FromQuery] string category = "")
        {
            var count = await _notificationService.GetUnreadCountAsync(category, recipientId);
            return Ok(count);
        }

        /// <summary>
        /// Belirtilen bildirimi "Okundu" olarak işaretler.
        /// </summary>
        /// <param name="id">Okundu olarak işaretlenecek bildirimin ID'si.</param>
        /// <param name="recipientId">Güvenlik kontrolü: Bildirimin gerçekten bu kişiye ait olup olmadığını doğrulamak için gönderilen Kullanıcı/Mekan ID'si.</param>
        [HttpPatch("{id:guid}/read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MarkAsRead(Guid id, [FromQuery] Guid recipientId)
        {
            await _notificationService.MarkAsReadAsync(id, recipientId);
            return NoContent();
        }
    }
}