using CityDiscovery.AdminNotificationService.Application.Common.Models;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.DeleteAllNotifications;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.DeleteNotification;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.MarkAllAsRead;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.MarkAsRead;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.DTOs;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Queries.GetUnreadCount;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Queries.GetUserNotifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CityDiscovery.AdminNotificationService.API.Models.Requests; // Request modelleri için

namespace CityDiscovery.AdminNotificationService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// (User-0) Kullanıcının bildirim listesini getirir.
        /// </summary>
        [ProducesResponseType(typeof(PagedResult<NotificationDto>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult<PagedResult<NotificationDto>>> GetUserNotifications(
            [FromQuery] Guid userId, // <-- MANUEL GİRİŞ GERİ GELDİ
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] bool unreadOnly = false,
            CancellationToken cancellationToken = default)
        {
            var query = new GetUserNotificationsQuery
            {
                UserId = userId,
                Page = page,
                PageSize = pageSize,
                UnreadOnly = unreadOnly
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// (User-0) Kullanıcının toplam okunmamış bildirim sayısını döner.
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount(
            [FromQuery] Guid userId, // <-- MANUEL GİRİŞ GERİ GELDİ
            CancellationToken cancellationToken = default)
        {
            var query = new GetUnreadCountQuery(userId);
            var count = await _mediator.Send(query, cancellationToken);
            return Ok(count);
        }

        /// <summary>
        /// (User-0) Belirli bir bildirimi okundu olarak işaretler.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead(
            Guid id,
            [FromBody] MarkReadRequest request, // <-- BODY'DEN ID ALMA GERİ GELDİ
            CancellationToken cancellationToken = default)
        {
            var command = new MarkNotificationAsReadCommand
            {
                NotificationId = id,
                UserId = request.UserId
            };

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// (User-0) Kullanıcının tüm bildirimlerini tek seferde 'okundu' olarak işaretler.
        /// </summary>
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead(
            [FromBody] MarkAllReadRequest request, // <-- BODY'DEN ID ALMA GERİ GELDİ
            CancellationToken cancellationToken = default)
        {
            var command = new MarkAllNotificationsAsReadCommand
            {
                UserId = request.UserId
            };

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        // DELETE api/notifications/{id}
        /// <summary>
        /// (User-0) Belirli bir bildirimi siler.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNotification(
            Guid id,
            [FromQuery] Guid userId, // <-- SİLME İŞLEMİ İÇİN QUERY'DEN ID İSTİYORUZ
            CancellationToken cancellationToken = default)
        {
            var command = new DeleteNotificationCommand
            {
                NotificationId = id,
                UserId = userId
            };

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        // DELETE api/notifications/delete-all
        /// <summary>
        /// (User-0) Kullanıcının tüm bildirimlerini siler (Temizlik).
        /// </summary>
        [HttpDelete("delete-all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAllNotifications(
            [FromQuery] Guid userId, // <-- SİLME İŞLEMİ İÇİN QUERY'DEN ID İSTİYORUZ
            CancellationToken cancellationToken = default)
        {
            var command = new DeleteAllNotificationsCommand
            {
                UserId = userId
            };

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}