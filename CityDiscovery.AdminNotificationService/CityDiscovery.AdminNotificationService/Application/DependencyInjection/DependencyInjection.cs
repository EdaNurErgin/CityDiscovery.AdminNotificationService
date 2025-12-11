using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
