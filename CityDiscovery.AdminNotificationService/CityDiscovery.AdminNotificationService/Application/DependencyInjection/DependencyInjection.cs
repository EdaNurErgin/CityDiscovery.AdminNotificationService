using System.Reflection;
using FluentValidation;


namespace CityDiscovery.AdminNotificationService.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAdminNotificationApplication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // CQRS (MediatR)
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            // FluentValidation (Command/Query validator'ları için)
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // AutoMapper kullanacaksan aç:
            // services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
