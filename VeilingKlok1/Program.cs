using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VeilingKlokApp.Data;
using VeilingKlokApp.Declarations;
using VeilingKlokApp.Services;

// Create the application builder
var builder = WebApplication.CreateBuilder(args);

#region Service Configuration

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRouting();

// Add Problem Details for standardized error responses
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
        context.ProblemDetails.Extensions.TryAdd("timestamp", DateTime.UtcNow);
    };
});

// Configure CORS to allow credentials (cookies)
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:3000", "http://localhost:5173") // Add your frontend URLs
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // Required for cookies
        }
    );
});

#endregion

#region Swagger Configuration

// Add Swagger services with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "VeilingKlok API",
            Version = "v1",
            Description = "Auction Clock API with JWT Authentication",
        }
    );

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
        }
    );

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                Array.Empty<string>()
            },
        }
    );
});

#endregion

#region Database Configuration

// Register the DbContext with SQL Server provider
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

#endregion

#region JWT Configuration

// Configure JWT Settings from appsettings.json
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

// Register custom services
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Database seeder (development only)
builder.Services.AddTransient<DatabaseSeeder>();

// Configure JWT Authentication
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Set to true in production
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(jwtSettings?.Secret ?? "")
            ),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings?.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings?.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero, // No tolerance for token expiration
        };
    });

#endregion

#region Application Build

// Build server application
var app = builder.Build();

// Run database seeder in development environment (adds dummy data)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    // Run synchronously during startup
    seeder.SeedAsync().GetAwaiter().GetResult();
}

#endregion

#region Middleware Pipeline

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Floriday V1 API");
    });
}

// Only redirect to HTTPS in production environments
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Enable CORS
app.UseCors("AllowFrontend");

// Authentication & Authorization middleware (order is important!)
app.UseAuthentication(); // Must come before UseAuthorization
app.UseAuthorization();
app.UseRouting();
app.MapControllers();

#endregion

#region Root Endpoint

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
                string htmlContent = await File.ReadAllTextAsync(fullPath);

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

// Run the application
app.Run();
