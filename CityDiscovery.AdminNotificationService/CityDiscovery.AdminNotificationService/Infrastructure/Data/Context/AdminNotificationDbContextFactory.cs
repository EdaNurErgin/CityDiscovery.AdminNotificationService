using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using CityDiscovery.AdminNotificationService.Domain.Entities;


namespace CityDiscovery.AdminNotificationService.Infrastructure.Data.Context
{
    // EF Core CLI (dotnet ef) çalışırken DbContext'i oluşturmaya yarar
    public class SocialDbContextFactory : IDesignTimeDbContextFactory<AdminNotificationDbContext>
    {
        public AdminNotificationDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

            var basePath = Directory.GetCurrentDirectory(); // .csproj'in çalıştığı klasör
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var connStr = config.GetConnectionString("DefaultConnection")
                         ?? throw new InvalidOperationException("Connection string 'DefaultConnection' bulunamadı.");

            var options = new DbContextOptionsBuilder<AdminNotificationDbContext>()
                .UseSqlServer(connStr)
                .Options;

            return new AdminNotificationDbContext(options);
        }
    }
}