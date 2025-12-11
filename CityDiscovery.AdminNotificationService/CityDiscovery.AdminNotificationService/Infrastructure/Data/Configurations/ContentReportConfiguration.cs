using CityDiscovery.AdminNotificationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityDiscovery.AdminNotificationService.Infrastructure.Data.Configurations
{
    public class ContentReportConfiguration : IEntityTypeConfiguration<ContentReport>
    {
        public void Configure(EntityTypeBuilder<ContentReport> builder)
        {
            builder.ToTable("ContentReports", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReportedType)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(x => x.Status)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(x => x.Reason)
                   .HasMaxLength(500);

            builder.HasIndex(x => new { x.ReportedType, x.ReportedId, x.Status })
                   .HasDatabaseName("IX_ContentReports_Target");

            builder.HasIndex(x => new { x.Status, x.CreatedAt })
                   .HasDatabaseName("IX_ContentReports_Status");
        }
    }
}
