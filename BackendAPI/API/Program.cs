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
            .AllowCredentials()
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

// ===========================
// IDENTITY API
// ===========================
app.MapGroup("/api/account")
    .MapIdentityApi<Domain.Entities.Account>()
    .RequireCors("AllowFrontend")
    .AddEndpointFilter(async (context, next) =>
    {
        var result = await next(context);

        if (context.HttpContext.Request.Path.Value?.EndsWith("/login",
                StringComparison.OrdinalIgnoreCase) == true)
        {
            if (result is Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult problem &&
                problem.StatusCode == 401)
            {
                if (problem.ProblemDetails?.Detail == "LockedOut")
                    throw CustomException.AccountLocked();

                throw CustomException.InvalidCredentials();
            }
        }

        return result;
    });

// ===========================
// SIGNALR
// ===========================
app.MapHub<VeilingHub>("/hubs/veiling-klok")
   .RequireCors("AllowFrontend");

// ===========================
// AUTO MIGRATE ONLY DEV
// ===========================
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
