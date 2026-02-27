using CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Commands.UpdateFeedbackStatus;
using CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Commands.CreateFeedback;
using CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Queries.GetOpenFeedback;
using CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Queries.GetUserFeedback;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CityDiscovery.AdminNotificationService.API.Models.Requests;
using Microsoft.AspNetCore.Authorization; 
using System.Security.Claims; 
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CityDiscovery.AdminNotificationService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Token'ın okunabilmesi ve güvenlik için eklendi
    public class FeedbackController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FeedbackController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST api/feedback
        /// <summary>
        /// Yeni bir geri bildirim (Hata, Öneri, Şikayet vb.) oluşturur.
        /// </summary>
        /// <remarks>
        /// Type alanına şunlar yazılabilir: 'Feedback', 'Complaint', 'Suggestion', 'Bug'.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> CreateFeedback(
            [FromBody] CreateFeedbackCommand command, 
            CancellationToken cancellationToken)
        {
            // TOKENDEN OTOMATİK ALMA: Geri bildirimi oluşturan kullanıcının ID'sini çekiyoruz
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out Guid tokenUserId))
            {
                if (command != null)
                {
                    command.UserId = tokenUserId; // Body'den gelen sahte ID'yi gerçek token ID'si ile eziyoruz
                }
            }

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetUserFeedback), new { userId = result.UserId }, result);
        }

        /// <summary>
        /// Bir kullanıcının geçmişte gönderdiği tüm geri bildirimleri listeler.
        /// </summary>
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetUserFeedback(
            Guid userId, // <-- ROUTE PARAMETRESİ KORUNDU (Akışı bozmamak için)
            CancellationToken cancellationToken)
        {
            /* Not: Bu metotta "userId" URL'den (route) geliyor. Adminlerin başka kullanıcıların 
               geçmiş geri bildirimlerini inceleyebilmesi (iş akışının bozulmaması) adına 
               burada Token ile ID ezme (override) işlemi bilerek yapılmamıştır. 
               Query eskisi gibi çalışmaya devam edecektir. */

            var query = new GetUserFeedbackQuery(userId);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Admin Paneli: Henüz çözümlenmemiş (Open veya InProgress) geri bildirimleri getirir.
        /// </summary>
        [HttpGet("open")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOpenFeedback(CancellationToken cancellationToken)
        {
            var query = new GetOpenFeedbackQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        // PUT api/feedback/{id}/status
        /// <summary>
        /// Admin Paneli: Geri bildirimin durumunu günceller.
        /// </summary>
        /// <remarks>
        /// NewStatus alanına şunlar yazılabilir: 'Open', 'InProgress', 'Resolved', 'Closed'.
        /// </remarks>
        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(
            Guid id,
            [FromBody] UpdateStatusRequest request, // <-- BODY KORUNDU
            CancellationToken cancellationToken)
        {
            // TOKENDEN OTOMATİK ALMA: İşlemi gerçekleştiren Admin'in ID'sini çekiyoruz
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(adminIdClaim) && Guid.TryParse(adminIdClaim, out Guid tokenAdminId))
            {
                if (request != null)
                {
                    request.AdminUserId = tokenAdminId; // Body'den gelen ID'yi güvenli Token ID'si ile eziyoruz
                }
            }

            var command = new UpdateFeedbackStatusCommand
            {
                FeedbackId = id,
                AdminUserId = request?.AdminUserId ?? Guid.Empty, // Token'dan alınan değer buraya geçer
                NewStatus = request?.NewStatus
            };

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}