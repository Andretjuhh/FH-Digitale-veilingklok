using Application.Repositories;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class RepositoryExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IKoperRepository, KoperRepository>();
        services.AddScoped<IKwekerRepository, KwekerRepository>();
        services.AddScoped<IMeesterRepository, MeesterRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IVeilingKlokRepository, VeilingKlokRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        return services;
    }
}
