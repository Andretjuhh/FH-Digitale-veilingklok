using System.Text;
using Application.Common.Exceptions;
using Application.Repositories;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Extensions;
using Infrastructure.MicroServices.Security;
using Infrastructure.MicroServices.Security.Configurations;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // This makes HttpContext available throughout your application's request pipeline.
        services.AddHttpContextAccessor();

        // Get app.json configurations
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddAuthenticationSecurity(configuration);

        // Dependency Injection for Infrastructure Repositories & Microservices
        services.AddRepositories();
        services.AddRealTimeServices();

        // Configure DbContext with SQL Server and specify the Migrations Assembly
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Infrastructure"))); // This is crucial!
        return services;
    }
}