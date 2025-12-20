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
        services.AddScoped<ITokenService, TokenService>();

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
            });

        services.AddAuthorization();
        return services;
    }
}
