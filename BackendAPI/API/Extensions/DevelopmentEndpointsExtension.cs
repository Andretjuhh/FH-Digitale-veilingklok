using Infrastructure.Persistence.Seeders;

namespace API.Extensions;

public static class DevelopmentEndpointsExtension
{
    /// <summary>
    /// Maps development-only endpoints for testing and seeding data.
    /// Only available when running in Development environment.
    /// </summary>
    public static WebApplication MapDevelopmentEndpoints(this WebApplication app)
    {
        app.MapGet(
                "/api/dev/seed-test-data",
                async (ITestDataSeeder seeder) =>
                {
                    try
                    {
                        await seeder.SeedAsync();
                        return Results.Ok(
                            new { success = true, message = "Test data seeded successfully!" }
                        );
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem(
                            ex.Message,
                            statusCode: 500,
                            title: "Seeding failed"
                        );
                    }
                }
            )
            .WithName("SeedTestData")
            .WithTags("Development")
            .WithDescription(
                "Seeds the database with test data including kwekers, kopers, products, and veilingklokken"
            );

        app.MapGet(
                "/api/dev/clear-test-data",
                async (ITestDataSeeder seeder) =>
                {
                    try
                    {
                        await seeder.ClearAllDataAsync();
                        return Results.Ok(
                            new { success = true, message = "Test data cleared successfully!" }
                        );
                    }
                    catch (Exception ex)
                    {
                        return Results.Problem(
                            ex.Message,
                            statusCode: 500,
                            title: "Clearing failed"
                        );
                    }
                }
            )
            .WithName("ClearTestData")
            .WithTags("Development")
            .WithDescription("Clears all test data from the database");

        // Log the available endpoints
        var urls = app.Urls.FirstOrDefault() ?? "http://localhost:5000";
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("==============================================");
        logger.LogInformation("Development Endpoints Available:");
        logger.LogInformation("Seed Data:        {SeedUrl}", $"{urls}/api/dev/seed-test-data");
        logger.LogInformation("Clear Seed Data:  {ClearUrl}", $"{urls}/api/dev/clear-test-data");
        logger.LogInformation("==============================================");

        return app;
    }
}