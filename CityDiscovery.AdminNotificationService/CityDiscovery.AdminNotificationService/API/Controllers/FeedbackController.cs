using CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Commands.UpdateFeedbackStatus;
using CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Commands.CreateFeedback;
using CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Queries.GetOpenFeedback;
using CityDiscovery.AdminNotificationService.Application.Features.UserFeedbacks.Queries.GetUserFeedback;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CityDiscovery.AdminNotificationService.API.Models.Requests;

namespace CityDiscovery.AdminNotificationService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            // JWT kullanırsan UserId'yi token'dan alıp command.UserId'ye set edebilirsin
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetUserFeedback), new { userId = result.UserId }, result);
        }

        /// <summary>
        /// Bir kullanıcının geçmişte gönderdiği tüm geri bildirimleri listeler.
        /// </summary>
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetUserFeedback(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var query = new GetUserFeedbackQuery(userId);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Admin Paneli: Henüz çözümlenmemiş (Open veya InProgress) geri bildirimleri getirir.
        /// </summary>
        [HttpGet("open")]
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
        public async Task<IActionResult> UpdateStatus(
            Guid id,
            [FromBody] UpdateStatusRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateFeedbackStatusCommand
            {
                FeedbackId = id,
                AdminUserId = request.AdminUserId,
                NewStatus = request.NewStatus
            };

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }


    }
}
