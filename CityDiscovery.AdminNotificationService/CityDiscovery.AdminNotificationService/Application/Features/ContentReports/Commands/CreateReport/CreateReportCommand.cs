using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.DTOs;
using MediatR;

namespace CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Commands.CreateReport
{
    public class CreateReportCommand : IRequest<ContentReportDto>
    {
        public string ReportedType { get; set; } = default!;  // Post, Comment, Photo, Venue, Review
        public Guid ReportedId { get; set; }
        public Guid ReportingUserId { get; set; }
        public string? Reason { get; set; }
    }
}
