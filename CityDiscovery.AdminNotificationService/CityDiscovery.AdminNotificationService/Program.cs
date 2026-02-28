using System.Reflection;
using CityDiscovery.AdminNotificationService.Application.DependencyInjection;
using CityDiscovery.AdminNotificationService.Infrastructure.DependencyInjection;
using Microsoft.OpenApi.Models;
using CityDiscovery.AdminNotificationService.API.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CityDiscovery.AdminNotificationService.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CityDiscovery.AdminNotificationService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.SetIsOriginAllowed(_ => true)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // Swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CityDiscovery Admin & Notification API",
                    Version = "v1"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT token'ınızı yapıştırın."
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

            builder.Services.AddHealthChecks();
            builder.Services.AddSignalR();

            
            //  SignalR için JWT query string desteği
            // SignalR WebSocket bağlantısı Header yerine query string'den token okur
            
            builder.Services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    var existingOnMessageReceived = options.Events?.OnMessageReceived;

                    options.Events ??= new JwtBearerEvents();
                    options.Events.OnMessageReceived = async context =>
                    {
                        // Önce varsa mevcut handler'ı çağır
                        if (existingOnMessageReceived != null)
                            await existingOnMessageReceived(context);

                        // SignalR hub path'leri için query string'den token al
                        var path = context.HttpContext.Request.Path;
                        var accessToken = context.Request.Query["access_token"];

                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/hubs/notifications") ||
                             path.StartsWithSegments("/hubs/user-notifications")))
                        {
                            context.Token = accessToken;
                        }
                    };
                });

            builder.Services.AddHealthChecks(); 

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<AdminNotificationDbContext>();
                    dbContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating the social database.");
                }
            }

            // Hub mapping'leri UseAuthentication'dan ÖNCE değil, SONRA olmalı
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Admin Notification API V1");
                    c.DocumentTitle = "CityDiscovery Frontend API Guide";
                    c.DefaultModelsExpandDepth(-1);
                });
            }

            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");         // 1. CORS
            app.UseAuthentication();           // 2. Authentication
            app.UseAuthorization();            // 3. Authorization
            app.MapHealthChecks("/health"); 
            app.MapControllers();

            // Hub mapping'leri — UseAuthorization'dan sonra
            app.MapHub<NotificationHub>("/hubs/notifications");
            app.MapHub<UserNotificationHub>("/hubs/user-notifications");
            
            app.Run();
        }
    }
}