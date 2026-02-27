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
    /// Admin bildirimleri
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid GetTokenUserId()
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Token user id bulunamadı.");

            return userId;
        }

        /// <summary>
        /// Kullanıcının bildirim listesini getirir.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<NotificationDto>>> GetUserNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] bool unreadOnly = false,
            CancellationToken cancellationToken = default)
        {
            var userId = GetTokenUserId();

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
        /// Kullanıcının toplam okunmamış bildirim sayısını döner.
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount(CancellationToken cancellationToken)
        {
            var userId = GetTokenUserId();

            var query = new GetUnreadCountQuery(userId);
            var count = await _mediator.Send(query, cancellationToken);

            return Ok(count);
        }

        /// <summary>
        /// Belirli bir bildirimi okundu olarak işaretler.
        /// </summary>
        [HttpPut("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var userId = GetTokenUserId();

            var command = new MarkNotificationAsReadCommand
            {
                NotificationId = id,
                UserId = userId
            };

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Tüm bildirimleri okundu yapar.
        /// </summary>
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
        {
            var userId = GetTokenUserId();

            var command = new MarkAllNotificationsAsReadCommand
            {
                UserId = userId
            };

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Belirli bir bildirimi siler.
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteNotification(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var userId = GetTokenUserId();

            var command = new DeleteNotificationCommand
            {
                NotificationId = id,
                UserId = userId
            };

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Tüm bildirimleri siler.
        /// </summary>
        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAllNotifications(CancellationToken cancellationToken)
        {
            var userId = GetTokenUserId();

            var command = new DeleteAllNotificationsCommand
            {
                UserId = userId
            };

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}