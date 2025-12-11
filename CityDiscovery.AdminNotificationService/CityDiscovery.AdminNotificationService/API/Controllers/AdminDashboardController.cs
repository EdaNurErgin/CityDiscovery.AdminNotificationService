using CityDiscovery.AdminNotificationService.Application.Features.AdminDashboard.DTOs;
using CityDiscovery.AdminNotificationService.Application.Features.AdminDashboard.Queries.GetAdminDashboardSummary;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CityDiscovery.AdminNotificationService.API.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminDashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/admin/dashboard?adminUserId=...
        [HttpGet]
        public async Task<ActionResult<AdminDashboardSummaryDto>> GetDashboard(
            [FromQuery] Guid adminUserId,
            CancellationToken cancellationToken)
        {
            // Gerçekte adminUserId'yi JWT claim'den okuyabilirsin:
            // var adminUserId = User.GetUserId();

            var query = new GetAdminDashboardSummaryQuery(adminUserId);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
    }
}
