using CityDiscovery.AdminNotificationService.Application.Features.ContentReports.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace CityDiscovery.AdminNotificationService.Application.Features.ContentReports.Commands.CreateReport
{
    public class CreateReportCommand : IRequest<ContentReportDto>
    {
        public string ReportedType { get; set; } = default!;  // Post, Comment, Photo, Venue, Review
        public Guid ReportedId { get; set; }
        [JsonIgnore] 
        public Guid ReportingUserId { get; set; }
        public string? Reason { get; set; }
    }
}
