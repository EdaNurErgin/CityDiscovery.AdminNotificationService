using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CityDiscovery.AdminNotificationService.Infrastructure.Security
{
    public static class JwtConfiguration
    {
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"];
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;

                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ClockSkew = TimeSpan.Zero,

                    NameClaimType = "sub",  // ← ClaimTypes.NameIdentifier değil, "sub"
                };

                x.Events = new JwtBearerEvents
                {
                    // SignalR WebSocket bağlantıları için query string'den token oku
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/hubs/notifications") ||
                             path.StartsWithSegments("/hubs/user-notifications")))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },

                    // Token doğrulandıktan sonra "sub" claim'ini NameIdentifier'a kopyala
                    // Bu sayede SignalR'ın Clients.User(userId) çağrısı çalışır
                    OnTokenValidated = context =>
                    {
                        var sub = context.Principal?.FindFirst("sub")?.Value
                               ?? context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                        if (!string.IsNullOrEmpty(sub))
                        {
                            // HttpContext.Items'a da ekle (diğer servisler için)
                            context.HttpContext.Items["UserId"] = Guid.TryParse(sub, out var guid) ? guid : Guid.Empty;

                            // Eğer NameIdentifier claim'i yoksa ekle
                            var identity = context.Principal?.Identity as ClaimsIdentity;
                            if (identity != null &&
                                identity.FindFirst(ClaimTypes.NameIdentifier) == null)
                            {
                                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, sub));
                            }
                        }

                        return Task.CompletedTask;
                    },

                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"[JWT FAILED] {context.Exception.GetType().Name}: {context.Exception.Message}");
                        Console.WriteLine($"[JWT FAILED] Inner: {context.Exception.InnerException?.Message}");
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<JwtBearerEvents>>();
                        logger.LogError("JWT doğrulama başarısız: {Error}", context.Exception.Message);
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}