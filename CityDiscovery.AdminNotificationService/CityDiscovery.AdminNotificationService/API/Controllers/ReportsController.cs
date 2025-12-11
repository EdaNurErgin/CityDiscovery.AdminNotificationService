using CityDiscovery.AdminNotificationService.Application.Common.Models;
using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Commands.CreateReport;
using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Commands.ResolveReport;
using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.DTOs;
using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Queries.GetContentReports;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CityDiscovery.AdminNotificationService.API.Models.Requests;


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

        // GET api/reports?page=1&pageSize=10&status=Open
        [HttpGet]
        public async Task<ActionResult<PagedResult<ContentReportDto>>> GetReports(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            CancellationToken cancellationToken = default)
        {
            var query = new GetContentReportsQuery
            {
                Page = page,
                PageSize = pageSize,
                Status = status
            };

            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

    }
}
