using System.Text.Json.Serialization;
using API.Extensions;
using Application;
using Infrastructure;
using Infrastructure.Microservices.SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddControllers();
builder.Services.AddRouting();
builder.Services.AddProblemsExtension();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:3000", "https://yourdomain.com")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // REQUIRED for SignalR
        }
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Floriday V1 API"); });
}

// Only redirect to HTTPS in production environments
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseExceptionHandler();
app.UseRouting();
app.UseAuthentication(); // Must come before UseAuthorization
app.UseAuthorization();
app.MapControllers();

// This creates the WebSocket endpoint at: ws://localhost:5000/hubs/veiling-klok
// Clients connect to this URL to establish real-time connection
app.MapHub<VeilingHub>("/hubs/veiling-klok");

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

app.Run();


// Note: For running dotnet
//  dotnet ef migrations add InitialCreate --project Infrastructure --startup-project API
//  dotnet ef database drop --project Infrastructure --startup-project API --force
//  dotnet ef database update --project Infrastructure --startup-project API
//  dotnet ef migrations remove --project Infrastructure --startup-project API