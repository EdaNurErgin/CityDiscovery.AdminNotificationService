using CityDiscovery.AdminNotificationService.Application.Common.Models; // PagedResult burada
using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.DTOs;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Queries.GetContentReports
{
    public class GetContentReportsQuery : IRequest<PagedResult<ContentReportDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Status { get; set; } // null gelirse hepsini getirir
    }
}