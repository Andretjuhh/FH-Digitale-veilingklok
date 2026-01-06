using Application.Services;
using Infrastructure.Microservices.SignalR.Interfaces;
using Infrastructure.Microservices.SignalR.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class SignalRExtension
{
    public static IServiceCollection AddRealTimeServices(this IServiceCollection services)
    {
        services.AddSignalR(options =>
        {
            // Configure SignalR options
            options.EnableDetailedErrors = true; // Shows detailed errors in development
            options.KeepAliveInterval = TimeSpan.FromSeconds(10); // Ping clients every 10s
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(30); // Timeout after 30s
            options.MaximumReceiveMessageSize = 32_000; // 32KB max message size
        });

        // StateManager holds in-memory state for ALL active clocks
        // If it was Scoped, each HTTP request would get a NEW instance and lose track of active clocks!
        // One StateManager for entire app → all clocks stored in one place
        services.AddSingleton<IVeilingKlokEngine, VeilingKlokEngine>();

        // Same for Notifier - needs to broadcast to persistent connections
        // One Notifier for entire app → broadcasts to all SignalR connections
        services.AddSingleton<IVeilingKlokNotifier, VeilingKlokNotifier>();

        return services;
    }
}