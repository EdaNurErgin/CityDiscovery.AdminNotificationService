using CityDiscovery.AdminNotificationService.Application.Interfaces.External;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Infrastructure.Data.Context;
using CityDiscovery.AdminNotificationService.Infrastructure.Data.Repositories;
using CityDiscovery.AdminNotificationService.Infrastructure.ExternalServices; // Bunu ekledik
using CityDiscovery.AdminNotificationService.Infrastructure.MessageBus.Consumers;
using CityDiscovery.AdminNotificationService.Infrastructure.Security;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace CityDiscovery.AdminNotificationService.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAdminNotificationInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 1. Veritabanı Bağlantısı
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AdminNotificationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // 2. Repository Kayıtları
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IUserFeedbackRepository, UserFeedbackRepository>();
            services.AddScoped<IContentReportRepository, ContentReportRepository>();
            services.AddScoped<IAdminAuditLogRepository, AdminAuditLogRepository>();
            services.AddJwtAuthentication(configuration);
            // 3. External Services (HTTP Clients) - EKSİKTİ, EKLENDİ
            // VenueCreatedConsumer içinde Identity servisine istek atıyoruz, bu yüzden gerekli.
            services.AddHttpClient<IIdentityServiceClient, IdentityServiceClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["ServiceUrls:IdentityService"] ?? "http://localhost:5001");
            });

            // 4. MassTransit + RabbitMQ (Tek blok halinde birleştirildi)
            services.AddMassTransit(x =>
            {
                // Tüm Consumer'ları buraya ekliyoruz
                x.AddConsumer<VenueCreatedConsumer>();
                x.AddConsumer<VenueApprovedConsumer>();
                x.AddConsumer<CommentAddedConsumer>();
                x.AddConsumer<PostLikedConsumer>();
                x.AddConsumer<ReviewCreatedConsumer>();
                // AddMassTransit bloğunun içine ekle:
                x.AddConsumer<ReviewDeletedConsumer>();
                x.AddConsumer<PostDeletedConsumer>();

                // Queue isimlerini otomatik formatla (örn: venue-created-event)
                x.SetKebabCaseEndpointNameFormatter();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var host = configuration["RabbitMQ:Host"] ?? "localhost";
                    var username = configuration["RabbitMQ:Username"] ?? "guest";
                    var password = configuration["RabbitMQ:Password"] ?? "guest";

                    cfg.Host(host, h =>
                    {
                        h.Username(username);
                        h.Password(password);
                    });

                    // Hata durumunda tekrar deneme politikası
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                    // Otomatik Endpoint Yapılandırması
                    // Bu metod, yukarıda eklediğimiz Consumer'lar için gerekli queue'ları 
                    // otomatik oluşturur ve bağlar. Tek tek ReceiveEndpoint yazmanıza gerek kalmaz.
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}