using CityDiscovery.AdminNotificationService.Application.Features.AdminDashboard.DTOs;
using CityDiscovery.AdminNotificationService.Application.Features.AdminDashboard.Queries.GetAdminDashboardSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CityDiscovery.AdminNotificationService.API.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize] // Güvenlik için Authorize eklendi
    public class AdminDashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminDashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Admin panelindeki özet sayıları (Toplam Feedback, Açık Raporlar, Okunmamış Bildirimler) döner.
        /// </summary>
        /// <returns>AdminDashboardSummaryDto objesi döner.</returns>
        [HttpGet]
        public async Task<ActionResult<AdminDashboardSummaryDto>> GetDashboard(CancellationToken cancellationToken)
        {
            // JWT token içerisinden admin kullanıcısının ID'sini alıyoruz
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid adminUserId))
            {
                return Unauthorized(new { error = "Admin kimliği doğrulanamadı." });
            }

            // Token'dan okunan adminUserId Query nesnesine gönderiliyor
            var query = new GetAdminDashboardSummaryQuery(adminUserId);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }
    }
}