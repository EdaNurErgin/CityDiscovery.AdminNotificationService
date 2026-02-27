using CityDiscovery.AdminNotificationService.Application.Interfaces.External;
using CityDiscovery.AdminNotificationService.Application.Interfaces.Repositories;
using CityDiscovery.AdminNotificationService.Infrastructure.Data.Context;
using CityDiscovery.AdminNotificationService.Infrastructure.Data.Repositories;
using CityDiscovery.AdminNotificationService.Infrastructure.ExternalServices; 
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
           
           
            // 3. External Services (HTTP Clients) 
            // VenueCreatedConsumer içinde Identity servisine istek atıyoruz, bu yüzden gerekli.
            services.AddHttpClient<IIdentityServiceClient, IdentityServiceClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["ServiceUrls:IdentityService"] ?? "http://localhost:5001");
            });

            // 4. MassTransit + RabbitMQ 
            services.AddMassTransit(x =>
            {
                // Tüm Consumer'ları buraya ekliyoruz
                x.AddConsumer<VenueCreatedConsumer>();
                x.AddConsumer<VenueApprovedConsumer>();
                x.AddConsumer<CommentAddedConsumer>();
                x.AddConsumer<PostLikedConsumer>();
                x.AddConsumer<ReviewCreatedConsumer>();
                x.AddConsumer<ReviewDeletedConsumer>();
                x.AddConsumer<PostDeletedConsumer>();
                x.AddConsumer<UserCreatedConsumer>();
                x.AddConsumer<VenueFavoritedConsumer>();

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

                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                    
                    

                    cfg.ReceiveEndpoint("user-created-queue", e =>
                    {
                        // ÖNEMLİ: Eğer namespace uyuşmazlığından korkuyorsan 
                        // exchange adını manuel olarak buraya bağlayabilirsin:
                        e.Bind("IdentityService.Shared.MessageBus.Identity:UserCreatedEvent");

                        e.ConfigureConsumer<UserCreatedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("comment-added-notification-queue", e =>
                    {
                        e.Bind("CityDiscovery.SocialService.SocialServiceShared.Common.Events.Social:CommentAddedEvent");
                        e.ConfigureConsumer<CommentAddedConsumer>(context);
                    });
                    // Diğer (Venue, Review vb.) otomatik isimlendirme ile devam etsin
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}