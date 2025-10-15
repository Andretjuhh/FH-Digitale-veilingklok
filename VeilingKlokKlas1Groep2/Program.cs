using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http; // Added for HttpContext, if needed, though often implicit
using System.IO; // <-- NEW: Required for reading files
using Microsoft.EntityFrameworkCore;

using VeilingKlokApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRouting();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>  c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" }));

// Register the DbContext with SQL Server provider
builder.Services.AddDbContext<VeilingKlokContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Build server application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    { 
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

// Server Middleware configuration 
// Re-enabled HTTPS redirection, as it was in the original code.
app.UseHttpsRedirection(); 
app.UseAuthorization();
app.UseRouting();
app.MapControllers();

// Restored the simple root endpoint.
app.MapGet("/", async context =>
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
        await context.Response.WriteAsync($"Error 404: Landing page file not found at {fullPath}");
    }

}).ExcludeFromDescription(); // Exclude this endpoint from the Swagger documentation

app.Run();
