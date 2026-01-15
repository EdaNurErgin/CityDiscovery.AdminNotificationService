using CityDiscovery.AdminNotificationService.Application.Common.Models;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.MarkAllAsRead;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.MarkAsRead;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.DTOs;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Queries.GetUnreadCount;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Queries.GetUserNotifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CityDiscovery.AdminNotificationService.API.Models.Requests;


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

        /// <summary>(User-0)
        /// Kullanıcının bildirim listesini getirir.
        /// </summary>
        /// <remarks>
        /// Örnek: GET /api/notifications?userId=...&amp;page=1&amp;pageSize=20
        /// </remarks>
        /// <param name="userId">Bildirimleri getirilecek kullanıcının benzersiz ID'si</param>
        /// <param name="page">Sayfa numarası (Varsayılan: 1)</param>
        /// <param name="pageSize">Sayfadaki kayıt sayısı (Varsayılan: 20)</param>
        /// <param name="unreadOnly">Sadece okunmamışları getirmek için true gönderin</param>

        [ProducesResponseType(typeof(PagedResult<NotificationDto>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<ActionResult<PagedResult<NotificationDto>>> GetUserNotifications(
            [FromQuery] Guid userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] bool unreadOnly = false,
            CancellationToken cancellationToken = default)
        {
            // NOT: Gerçekte userId'yi token'dan alman daha güvenli:
            // var userId = User.GetUserId();

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

        // GET api/notifications/unread-count?userId=...
        /// <summary>(User-0)
        /// Kullanıcının toplam okunmamış bildirim sayısını döner.
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount(
            [FromQuery] Guid userId,
            CancellationToken cancellationToken = default)
        {
            var query = new GetUnreadCountQuery(userId);
            var count = await _mediator.Send(query, cancellationToken);
            return Ok(count);
        }

        // PUT api/notifications/{id}/read
        /// <summary>(User-0)
        /// Belirli bir bildirimi okundu olarak işaretler.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead(
            Guid id,
            [FromBody] MarkReadRequest request,
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

        // PUT api/notifications/read-all
        /// <summary>(User-0)
        /// Kullanıcının tüm bildirimlerini tek seferde 'okundu' olarak işaretler.
        /// </summary>
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead(
            [FromBody] MarkAllReadRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new MarkAllNotificationsAsReadCommand
            {
                UserId = request.UserId
            };

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }


    }
}
