using System.Text;
using System.Text.Json.Serialization;
using Application.Common.Exceptions;
using Application.Repositories;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Extensions;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


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
        services.AddAuthentication().AddJwtBearer();


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
        services.AddIdentityCore<Account>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        
        return services;
    }
}
