
using CityDiscovery.AdminNotificationService.Application;
using CityDiscovery.AdminNotificationService.Application.DependencyInjection;
using CityDiscovery.AdminNotificationService.Infrastructure;
using CityDiscovery.AdminNotificationService.Infrastructure.DependencyInjection;
using Microsoft.OpenApi.Models;
namespace CityDiscovery.AdminNotificationService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

          

            builder.Services.AddControllers();
          
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CityDiscovery.AdminNotificationService", Version = "v1" });

                
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http, 
                    Scheme = "Bearer",              
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Lütfen sadece JWT token'?n?z? yap??t?r?n. (Bearer yazman?za GEREK YOK)"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            builder.Services
                .AddAdminNotificationApplication(builder.Configuration)
                .AddAdminNotificationInfrastructure(builder.Configuration);

            // Health Checks
            builder.Services.AddHealthChecks();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            // Health Check Endpoint
            app.MapHealthChecks("/health");

            app.MapControllers();

            app.Run();
        }
    }
}

