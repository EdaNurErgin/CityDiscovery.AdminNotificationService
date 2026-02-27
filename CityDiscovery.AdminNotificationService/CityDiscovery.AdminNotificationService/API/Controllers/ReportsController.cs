using CityDiscovery.AdminNotificationService.Application.Common.Models;
using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Commands.CreateReport;
using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Commands.ResolveReport;
using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.DTOs;
using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Queries.GetContentReports;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CityDiscovery.AdminNotificationService.API.Models.Requests;
using Microsoft.AspNetCore.Authorization; // Token için eklendi
using System.Security.Claims; // ClaimTypes için eklendi
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CityDiscovery.AdminNotificationService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Güvenlik ve Token'ın okunabilmesi için eklendi
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST api/reports
        /// <summary>(admin)
        /// Bir içeriği (Post, Comment, Venue vb.) raporlar/şikayet eder.
        /// </summary>
        /// <remarks>
        /// ReportedType: 'Post', 'Comment', 'Photo', 'Venue', 'Review'.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> CreateReport(
            [FromBody] CreateReportCommand command, // <-- BODY KORUNDU
            CancellationToken cancellationToken)
        {
            // TOKENDEN OTOMATİK ALMA: Şikayeti oluşturan kullanıcının ID'sini token'dan çekiyoruz
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out Guid tokenUserId))
            {
                if (command != null)
                {
                    command.ReportingUserId = tokenUserId; // Body'den gelen ID'yi güvenli token ID'si ile eziyoruz
                }
            }

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(CreateReport), new { id = result.Id }, result);
        }

        // PUT api/reports/{id}/resolve
        /// <summary>(admin)
        /// Admin Paneli: Raporu sonuçlandırır (Onayla veya Reddet).
        /// </summary>
        /// <remarks>
        /// NewStatus: 'Resolved' (İçerik kaldırıldı/onaylandı) veya 'Rejected' (Şikayet reddedildi).
        /// </remarks>
        [HttpPut("{id:guid}/resolve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResolveReport(
            Guid id,
            [FromBody] ResolveReportRequest request, 
            CancellationToken cancellationToken)
        {
            // TOKENDEN OTOMATİK ALMA: İşlemi yapan Admin'in ID'sini token'dan çekiyoruz
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(adminIdClaim) && Guid.TryParse(adminIdClaim, out Guid tokenAdminId))
            {
                if (request != null)
                {
                    request.AdminUserId = tokenAdminId; // Request'ten gelen ID'yi token'dan gelen güvenli admin ID'si ile eziyoruz
                }
            }

            var command = new ResolveReportCommand
            {
                ReportId = id,
                AdminUserId = request?.AdminUserId ?? Guid.Empty,
                NewStatus = request?.NewStatus
            };

            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        // GET api/reports?page=1&pageSize=10&status=Open
        /// <summary>(admin)
        /// Admin Paneli: Tüm raporları sayfalı ve filtreli olarak listeler.
        /// </summary>
        /// <param name="status">Opsiyonel filtre: 'Open', 'Resolved', 'Rejected'.</param>
        [HttpGet]
        [Authorize(Roles = "Admin")]
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