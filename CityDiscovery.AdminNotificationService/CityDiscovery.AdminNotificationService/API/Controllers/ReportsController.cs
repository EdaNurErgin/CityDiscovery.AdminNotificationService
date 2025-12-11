using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Commands.CreateReport;
using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Commands.ResolveReport;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CityDiscovery.AdminNotificationService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST api/reports
        [HttpPost]
        public async Task<IActionResult> CreateReport(
            [FromBody] CreateReportCommand command,
            CancellationToken cancellationToken)
        {
            // İleride JWT'den UserId alırsın:
            // var userId = User.GetUserId(); vs.
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(CreateReport), new { id = result.Id }, result);
        }

        // PUT api/reports/{id}/resolve
        [HttpPut("{id:guid}/resolve")]
        public async Task<IActionResult> ResolveReport(
            Guid id,
            [FromBody] ResolveReportRequest request,
            CancellationToken cancellationToken)
        {
            var command = new ResolveReportCommand
            {
                ReportId = id,
                AdminUserId = request.AdminUserId,
                NewStatus = request.NewStatus
            };

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        public class ResolveReportRequest
        {
            public Guid AdminUserId { get; set; }
            public string NewStatus { get; set; } = default!; // "Resolved" veya "Rejected"
        }
    }
}
