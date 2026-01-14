using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Application.Common.Exceptions;
using Application.Services;
using Infrastructure.MicroServices.Security;
using Infrastructure.MicroServices.Security.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Extensions;

public static class AuthenticationExtension
{
    public static IServiceCollection AddAuthenticationSecurity(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Retrieve JWT settings from configuration
        var jwtSettings =
            configuration.GetSection(JwtConfiguration.Key).Get<JwtConfiguration>()
            ?? throw CustomException.MissingJwtConfiguration();

        services.Configure<JwtConfiguration>(configuration.GetSection(JwtConfiguration.Key));

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Store in HttpContext for later retrieval
                options.SaveToken = true;
                options.RequireHttpsMetadata = false; // Set to true in production
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // This is where the framework reads the configuration to validate tokens
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    // Settings from appsettings.json
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Secret)
                    ),
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger =
                            context.HttpContext.RequestServices.GetRequiredService<Microsoft.Extensions.Logging.ILogger<JwtBearerEvents>>();

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger =
                            context.HttpContext.RequestServices.GetRequiredService<Microsoft.Extensions.Logging.ILogger<JwtBearerEvents>>();
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;

                        // Add logging to debug
                        var logger =
                            context.HttpContext.RequestServices.GetRequiredService<Microsoft.Extensions.Logging.ILogger<JwtBearerEvents>>();

                        if (
                            !string.IsNullOrEmpty(accessToken)
                            && (path.StartsWithSegments("/hubs/veiling-klok"))
                        )
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    },
                };
            });

        services.AddScoped<ITokenService, TokenService>();
        services.AddAuthorization();
        return services;
    }
}
