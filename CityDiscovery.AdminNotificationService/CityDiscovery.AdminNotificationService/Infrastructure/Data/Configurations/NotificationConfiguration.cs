using CityDiscovery.AdminNotificationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityDiscovery.AdminNotificationService.Infrastructure.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Type)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.Payload)
                   .HasMaxLength(2000);

            builder.Property(x => x.TargetType)
                   .HasMaxLength(20);

            builder.Property(x => x.Route)
                   .HasMaxLength(300);

            builder.HasIndex(x => new { x.UserId, x.IsRead, x.CreatedAt })
                   .HasDatabaseName("IX_Notifications_User_IsRead");

            builder.HasIndex(x => new { x.TargetType, x.TargetId, x.CreatedAt })
                   .HasDatabaseName("IX_Notifications_Target");
        }
    }
}
