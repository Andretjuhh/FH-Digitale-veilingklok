using API.Extensions;
using Application;
using Application.Common.Exceptions;
using Infrastructure;
using Infrastructure.Microservices.SignalR.Hubs;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===========================
// FORCE AZURE CONFIG LOADING
// ===========================
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// ===========================
// LOGGING
// ===========================
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ===========================
// KESTREL PORT FIX FOR AZURE
// ===========================
builder.WebHost.ConfigureKestrel(options =>
{
    var port = Environment.GetEnvironmentVariable("PORT");
    if (!string.IsNullOrEmpty(port))
        options.ListenAnyIP(int.Parse(port));
});

// ===========================
// CONNECTION STRING VALIDATION
// ===========================
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection") ??
    builder.Configuration["ConnectionStrings:DefaultConnection"];

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("❌ DefaultConnection not found. Check Azure → Configuration → Connection strings");
}

// ===========================
// SERVICE REGISTRATION
// ===========================
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddControllers();
builder.Services.AddProblemsExtension();

// ===========================
// CORS
// ===========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "https://yourdomain.com",
                "https://veilingklok-frontendklas1groep2-hvcmg3eua4fhgqft.germanywestcentral-01.azurewebsites.net"
            )
            .AllowAnyHeader()
            .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS")
<<<<<<< Updated upstream
            .AllowCredentials()
=======
            .AllowCredentials() // REQUIRED for SignalR and credentials: 'include'
>>>>>>> Stashed changes
            .WithExposedHeaders("Content-Disposition", "Content-Length")
            .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
});

var app = builder.Build();

// ===========================
// PIPELINE
// ===========================
app.UseExceptionHandler();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSwaggerDocumentation();

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

<<<<<<< Updated upstream
// ===========================
// SIGNALR
// ===========================
app.MapHub<VeilingHub>("/hubs/veiling-klok")
   .RequireCors("AllowFrontend");

// ===========================
// AUTO MIGRATE ONLY DEV
// ===========================
=======
<<<<<<< HEAD
// ONLY IN DEV
>>>>>>> Stashed changes
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
=======
// Identity API Endpoints
app.MapGroup("/api/account")
    .MapIdentityApi<Domain.Entities.Account>()
    .RequireCors("AllowFrontend")
    .AddEndpointFilter(
        async (context, next) =>
        {
            var result = await next(context);

            // The Path property does NOT include the QueryString, so EndsWith("/login") is safe
            // regardless of parameters like ?useCookies=true
            if (
                context.HttpContext.Request.Path.Value?.EndsWith(
                    "/login",
                    StringComparison.OrdinalIgnoreCase
                ) == true
            )
            {
                // Identity API returns Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>
                // We need to unwrap the actual result using reflection
                var resultType = result?.GetType();
                var resultProperty = resultType?.GetProperty("Result");
                var actualResult = resultProperty?.GetValue(result);

                // Check for ProblemDetails response (LockedOut, NotAllowed, InvalidCredentials, etc.)
                if (actualResult is Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult problem)
                    if (problem.StatusCode == 401)
                    {
                        if (problem.ProblemDetails.Detail == "LockedOut")
                            throw CustomException.AccountLocked();
                        else
                            // Other 401 problem details (NotAllowed, Failed, etc.)
                            throw CustomException.InvalidCredentials();
                    }

                // Fallback: Check for standard 401 status code
                if (actualResult is IStatusCodeHttpResult { StatusCode: 401 })
                    throw CustomException.InvalidCredentials();
            }

            return result;
        }
    );

// This creates the WebSocket endpoint at: ws://localhost:5000/hubs/veiling-klok
// Clients connect to this URL to establish real-time connection
app.MapHub<VeilingHub>("/hubs/veiling-klok").RequireCors("AllowFrontend");

// Map development endpoints (seeder, testing utilities, etc.)
app.MapDevelopmentEndpoints();

#region Home Page Routing

// Restored the simple root endpoint.
app.MapGet(
        "/",
        async context =>
        {
            // Define the relative path to the HTML file in the new 'Html' folder
            const string filePath = "wwwroot/landingPage.html";

            // Determine the full path relative to the application's Content Root Path
            var fullPath = Path.Combine(app.Environment.ContentRootPath, filePath);

            if (File.Exists(fullPath))
            {
                // Read the HTML content from the file
                var htmlContent = await File.ReadAllTextAsync(fullPath);

                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(htmlContent);
            }
            else
            {
                // Fallback error message if the file is not found
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync(
                    $"Error 404: Landing page file not found at {fullPath}"
                );
            }
        }
    )
    .ExcludeFromDescription(); // Exclude this endpoint from the Swagger documentation
#endregion
>>>>>>> main

app.Run();
<<<<<<< Updated upstream
=======


<<<<<<< HEAD
// // Note: For running dotnet ( !!!! Run In the same order always !!!! )
// //  dotnet ef database drop --project Infrastructure --startup-project API --force
// //  dotnet ef migrations remove --project Infrastructure --startup-project API
// //  dotnet ef migrations add InitialCreate --project Infrastructure --startup-project API
// //  dotnet ef database update --project Infrastructure --startup-project API
=======
// Note: For running dotnet ( !!!! Run In the same order always !!!! )
//  dotnet ef database drop --project Infrastructure --startup-project API --force
//  dotnet ef migrations remove --project Infrastructure --startup-project API
//  dotnet ef migrations add InitialCreate --project Infrastructure --startup-project API
//  dotnet ef database update --project Infrastructure --startup-project API
>>>>>>> main
>>>>>>> Stashed changes
