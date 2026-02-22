using CityDiscovery.AdminNotificationService.Application.Common.Models;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.DeleteAllNotifications;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.DeleteNotification;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.MarkAllAsRead;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.MarkAsRead;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.DTOs;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Queries.GetUnreadCount;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Queries.GetUserNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CityDiscovery.AdminNotificationService.API.Controllers
{
    /// <summary>
    /// Kullanıcı Bildirimleri API — Normal kullanıcılar kendi bildirimlerini bu endpoint üzerinden yönetir.
    /// Yorum, beğeni gibi sosyal etkileşim bildirimleri burada listelenir.
    /// </summary>
    [ApiController]
    [Route("api/user/notifications")]
    [Authorize]
    [Produces("application/json")]
    public class UserNotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserNotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Giriş yapmış kullanıcının bildirim listesini getirir.
        /// </summary>
        /// <param name="page">Sayfa numarası (varsayılan: 1)</param>
        /// <param name="pageSize">Sayfa başına bildirim sayısı (varsayılan: 20)</param>
        /// <param name="unreadOnly">Sadece okunmamışları getir (varsayılan: false)</param>
        /// <remarks>
        /// Token içindeki UserId kullanılır. Manuel olarak userId göndermeye gerek yoktur.
        /// 
        /// Bildirim Tipleri:
        /// - **NewComment**: Birileri postunuza yorum yaptı
        /// - **PostLiked**: Birisi postunuzu beğendi
        /// - **VenueApproved**: Mekanınız onaylandı
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<NotificationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PagedResult<NotificationDto>>> GetMyNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] bool unreadOnly = false,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { error = "Token geçersiz veya UserId bulunamadı." });

            var query = new GetUserNotificationsQuery
            {
                UserId = userId.Value,
                Page = page,
                PageSize = pageSize,
                UnreadOnly = unreadOnly
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Giriş yapmış kullanıcının okunmamış bildirim sayısını döner.
        /// </summary>
        /// <remarks>
        /// Mobil/web uygulamasında bildirim rozeti (badge) göstermek için kullanılır.
        /// </remarks>
        [HttpGet("unread-count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<int>> GetUnreadCount(
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { error = "Token geçersiz veya UserId bulunamadı." });

            var query = new GetUnreadCountQuery(userId.Value);
            var count = await _mediator.Send(query, cancellationToken);
            return Ok(count);
        }

        /// <summary>
        /// Belirli bir bildirimi okundu olarak işaretler.
        /// </summary>
        /// <param name="id">Bildirim ID'si</param>
        [HttpPut("{id:guid}/read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsRead(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { error = "Token geçersiz veya UserId bulunamadı." });

            var command = new MarkNotificationAsReadCommand
            {
                NotificationId = id,
                UserId = userId.Value
            };

            try
            {
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Tüm bildirimleri okundu olarak işaretler.
        /// </summary>
        [HttpPut("read-all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkAllAsRead(
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { error = "Token geçersiz veya UserId bulunamadı." });

            var command = new MarkAllNotificationsAsReadCommand
            {
                UserId = userId.Value
            };

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Belirli bir bildirimi siler.
        /// </summary>
        /// <param name="id">Silinecek bildirim ID'si</param>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNotification(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { error = "Token geçersiz veya UserId bulunamadı." });

            try
            {
                var command = new DeleteNotificationCommand
                {
                    NotificationId = id,
                    UserId = userId.Value
                };

                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        /// <summary>
        /// Tüm bildirimleri siler (Bildirim kutusunu temizler).
        /// </summary>
        [HttpDelete("clear-all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ClearAllNotifications(
            CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { error = "Token geçersiz veya UserId bulunamadı." });

            var command = new DeleteAllNotificationsCommand
            {
                UserId = userId.Value
            };

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        // ─── Yardımcı Metot ─────────────────────────────────────────────────────

        /// <summary>
        /// JWT token'dan UserId'yi okur.
        /// IdentityService'de token oluştururken ClaimTypes.NameIdentifier veya "sub" kullanılıyor olmalı.
        /// </summary>
        private Guid? GetCurrentUserId()
        {
            // Önce standart NameIdentifier claim'ini dene
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue("sub")
                         ?? User.FindFirstValue("userId");

            if (string.IsNullOrEmpty(userIdStr))
                return null;

            return Guid.TryParse(userIdStr, out var userId) ? userId : null;
        }
    }
}