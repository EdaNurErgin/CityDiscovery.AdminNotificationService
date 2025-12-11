using CityDiscovery.AdminNotificationService.Application.Common.Models;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.MarkAllAsRead;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Commands.MarkAsRead;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.DTOs;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Queries.GetUnreadCount;
using CityDiscovery.AdminNotificationService.Application.Features.Notifications.Queries.GetUserNotifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

        // GET api/notifications?userId=...&page=1&pageSize=20&unreadOnly=true
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

        public class MarkReadRequest
        {
            public Guid UserId { get; set; }
        }

        public class MarkAllReadRequest
        {
            public Guid UserId { get; set; }
        }
    }
}
