using API.Models;
using Application.DTOs.Input;
using Application.DTOs.Output;
using Application.UseCases.VeilingKlok;
using Domain.Entities;
using Domain.Interfaces;
using Domain.ValueObjects;
using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Seeders;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class DevelopmentEndpointsExtension
{
    /// <summary>
    /// Maps development-only endpoints for testing and seeding data.
    /// Only available when running in Development environment.
    /// </summary>
    public static WebApplication MapDevelopmentEndpoints(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
            return app;

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

        app.MapPost(
                "/api/dev/veilingklok/dummy",
                async context =>
                {
                    var request = await context.Request.ReadFromJsonAsync<DevVeilingKlokRequest>();
                    if (request == null)
                    {
                        await Results.BadRequest(new { message = "Request body is required." })
                            .ExecuteAsync(context);
                        return;
                    }

                    var dbContext = context.RequestServices.GetRequiredService<AppDbContext>();
                    var passwordHasher = context.RequestServices.GetRequiredService<IPasswordHasher>();
                    var mediator = context.RequestServices.GetRequiredService<IMediator>();

                    if (request.Products.Count == 0)
                    {
                        await Results.BadRequest(
                                new { message = "At least one product is required." }
                            )
                            .ExecuteAsync(context);
                        return;
                    }

                    var meester = await EnsureDevVeilingmeesterAsync(dbContext, passwordHasher);
                    var kwekersByCompany = new Dictionary<string, Kweker>(
                        StringComparer.OrdinalIgnoreCase
                    );

                    foreach (var product in request.Products)
                    {
                        var companyName = string.IsNullOrWhiteSpace(product.CompanyName)
                            ? "Dev Kweker"
                            : product.CompanyName.Trim();

                        if (!kwekersByCompany.TryGetValue(companyName, out var kweker))
                        {
                            kweker = await dbContext.Kwekers.FirstOrDefaultAsync(
                                k => k.CompanyName == companyName
                            );

                            if (kweker == null)
                            {
                                var email = await GenerateUniqueEmailAsync(
                                    dbContext,
                                    $"{Slugify(companyName)}@dev.local"
                                );
                                var password = Password.Create("Test1234!", passwordHasher);
                                kweker = new Kweker(email, password)
                                {
                                    KvkNumber = $"DEV-{Guid.NewGuid():N}".Substring(0, 12),
                                    CompanyName = companyName,
                                    FirstName = "Dev",
                                    LastName = "Kweker",
                                    Telephone = "+31 6 00000000"
                                };

                                await dbContext.Kwekers.AddAsync(kweker);
                                await dbContext.SaveChangesAsync();
                            }

                            kwekersByCompany[companyName] = kweker;
                        }

                        var existingProduct = await dbContext.Products.FirstOrDefaultAsync(
                            p => p.Id == product.Id
                        );

                        if (existingProduct != null)
                            continue;

                        var maxPrice = product.MaxPrice > 0 ? product.MaxPrice : 1m;
                        var minPrice = Math.Round(maxPrice * 0.6m, 2);
                        var stock = product.Stock > 0 ? product.Stock : 1;

                        var newProduct = new Product(minPrice, stock)
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Description = product.Description,
                            Dimension = product.Dimension,
                            ImageUrl = string.IsNullOrWhiteSpace(product.ImageUrl)
                                ? Base64Placeholder
                                : product.ImageUrl,
                            KwekerId = kwekersByCompany[companyName].Id
                        };

                        await dbContext.Products.AddAsync(newProduct);
                    }

                    await dbContext.SaveChangesAsync();

                    var dto = new CreateVeilingKlokDTO
                    {
                        ScheduledAt = request.ScheduledAt,
                        VeilingDurationMinutes = request.VeilingDurationMinutes,
                        Products = request.Products.ToDictionary(
                            p => p.Id,
                            p => p.MaxPrice
                        )
                    };

                    var result = await mediator.Send(
                        new CreateVeilingKlokCommand(dto, meester.Id)
                    );

                    var actionContext = new ActionContext(
                        context,
                        new RouteData(),
                        new ActionDescriptor()
                    );

                    await HttpSuccess<VeilingKlokDetailsOutputDto>
                        .Created(result, "Dev veilingklok created successfully")
                        .ExecuteResultAsync(actionContext);
                }
            )
            .WithName("CreateDevVeilingKlok")
            .WithTags("Development")
            .WithDescription("Creates a dev veilingklok and persists dummy products for testing.");

        // Log the available endpoints
        var urls = app.Urls.FirstOrDefault() ?? "http://localhost:5000";
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("==============================================");
        logger.LogInformation("Development Endpoints Available:");
        logger.LogInformation("Seed Data:        {SeedUrl}", $"{urls}/api/dev/seed-test-data");
        logger.LogInformation("Clear Seed Data:  {ClearUrl}", $"{urls}/api/dev/clear-test-data");
        logger.LogInformation("Dev VeilingKlok:  {DevVeilingUrl}", $"{urls}/api/dev/veilingklok/dummy");
        logger.LogInformation("==============================================");

        return app;
    }

    private static async Task<Veilingmeester> EnsureDevVeilingmeesterAsync(
        AppDbContext dbContext,
        IPasswordHasher passwordHasher
    )
    {
        const string email = "dev-veilingmeester@test.nl";
        var meester = await dbContext.Veilingmeesters.FirstOrDefaultAsync(
            m => m.Email == email
        );

        if (meester != null)
            return meester;

        var password = Password.Create("Test1234!", passwordHasher);
        var devMeester = new Veilingmeester(email, password)
        {
            CountryCode = "NL",
            Region = "Noord-Holland",
            AuthorisatieCode = "DEV-VM-001"
        };

        await dbContext.Veilingmeesters.AddAsync(devMeester);
        await dbContext.SaveChangesAsync();
        return devMeester;
    }

    private static async Task<string> GenerateUniqueEmailAsync(
        AppDbContext dbContext,
        string baseEmail
    )
    {
        var email = baseEmail;
        var suffix = 1;
        while (await dbContext.Accounts.AnyAsync(a => a.Email == email))
        {
            var parts = baseEmail.Split('@');
            email = $"{parts[0]}{suffix}@{parts[1]}";
            suffix++;
        }

        return email;
    }

    private static string Slugify(string value)
    {
        var chars = value
            .ToLowerInvariant()
            .Select(c => char.IsLetterOrDigit(c) ? c : '-')
            .ToArray();
        var slug = new string(chars).Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? "kweker" : slug;
    }

    private const string Base64Placeholder =
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==";

    public sealed record DevVeilingKlokRequest(
        DateTimeOffset ScheduledAt,
        int VeilingDurationMinutes,
        List<DevVeilingProductInput> Products
    );

    public sealed record DevVeilingProductInput(
        Guid Id,
        string Name,
        string Description,
        string? ImageUrl,
        string? Dimension,
        int Stock,
        string CompanyName,
        decimal MaxPrice
    );
}
