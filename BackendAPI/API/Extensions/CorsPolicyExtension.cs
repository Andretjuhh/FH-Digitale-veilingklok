namespace API.Extensions;

public static class CorsPolicyExtension
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowFrontend",
                policy =>
                {
                    // Get allowed origins from configuration
                    var origins = new List<string>
                    {
                        configuration.GetConnectionString("FrontendURL")!
                    };

                    if (environment.IsDevelopment())
                    {
                        origins.Add("http://localhost:3000");
                        origins.Add("http://localhost:5173");
                    }

                    policy
                        .WithOrigins(origins.ToArray())
                        .AllowAnyHeader()
                        .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS")
                        .AllowCredentials() // REQUIRED for SignalR and credentials: 'include'
                        .WithExposedHeaders("Content-Disposition", "Content-Length")
                        .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                }
            );
        });

        return services;
    }
}