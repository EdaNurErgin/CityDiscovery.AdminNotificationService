using System.Reflection;
using CityDiscovery.AdminNotificationService.Application.DependencyInjection;
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

            // Swagger Dokümantasyon Ayarlar?
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CityDiscovery Admin & Notification API",
                    Version = "v1",
                    Description = "Bu API; kullan?c? bildirimleri, sistem geri bildirimleri (Feedback) ve içerik raporlama (Reporting) süreçlerini yönetir. \n\n" +
                                  "**Not:** Tüm PUT/POST i?lemlerinde JSON gövdesi beklenmektedir."
                });

                // Kod içindeki /// <summary> yorumlar?n? Swagger'a aktar?r
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                // JWT Güvenlik Tan?m?
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Token'?n?z? buraya yap??t?r?n. Ba??ndaki 'Bearer ' k?sm?n? eklemenize gerek yoktur."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services
                .AddAdminNotificationApplication(builder.Configuration)
                .AddAdminNotificationInfrastructure(builder.Configuration);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Admin Notification API V1");
                    c.DocumentTitle = "CityDiscovery Frontend API Guide";
                    c.DefaultModelsExpandDepth(-1); // Model ?emalar?n? varsay?lan olarak kapal? tutar, kalabal??? önler
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}