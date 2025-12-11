using CityDiscovery.AdminNotificationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityDiscovery.AdminNotificationService.Infrastructure.Data.Context
{
    public class AdminNotificationDbContext : DbContext
    {
        public AdminNotificationDbContext(DbContextOptions<AdminNotificationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<ContentReport> ContentReports => Set<ContentReport>();
        public DbSet<UserFeedback> UserFeedbacks => Set<UserFeedback>();
        public DbSet<AdminAuditLog> AdminAuditLogs => Set<AdminAuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AdminNotificationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
