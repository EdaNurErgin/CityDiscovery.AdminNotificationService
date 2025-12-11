using CityDiscovery.AdminNotificationService.Domain.Entities;

namespace CityDiscovery.AdminNotificationService.Application.Features.ContentReports.DTOs
{
    public class ContentReportDto
    {
        public Guid Id { get; set; }
        public string ReportedType { get; set; } = default!;
        public Guid ReportedId { get; set; }
        public Guid ReportingUserId { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public Guid? ResolvedByUserId { get; set; }

        public static ContentReportDto FromEntity(ContentReport entity)
        {
            return new ContentReportDto
            {
                Id = entity.Id,
                ReportedType = entity.ReportedType,
                ReportedId = entity.ReportedId,
                ReportingUserId = entity.ReportingUserId,
                Reason = entity.Reason,
                Status = entity.Status,
                CreatedAt = entity.CreatedAt,
                ResolvedAt = entity.ResolvedAt,
                ResolvedByUserId = entity.ResolvedByUserId
            };
        }
    }
}
