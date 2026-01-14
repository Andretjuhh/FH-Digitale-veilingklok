using Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class AuthenticationExtension
{
    public static IServiceCollection AddAuthenticationSecurity(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddAuthorization();
        services.AddDataProtection();

        services
            .AddIdentityApiEndpoints<Account>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

        return services;
    }
}
