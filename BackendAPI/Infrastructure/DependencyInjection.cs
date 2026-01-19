using Application.Services;
using Infrastructure.Extensions;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Seeders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment
    )
    {
        // This makes HttpContext available throughout your application's request pipeline.
        services.AddHttpContextAccessor();
        services.AddAuthenticationSecurity(configuration, environment.IsDevelopment());

        // Get app.json configurations
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Test Data Seeder for Development
        services.AddScoped<ITestDataSeeder, TestDataSeeder>();

        // Dependency Injection for Infrastructure Repositories & Microservices
        services.AddRepositories();
        services.AddRealTimeServices();

        // Configure DbContext with SQL Server and specify the Migrations Assembly
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Infrastructure")
            )
        ); // This is crucial!
        return services;
    }
}