using API.Extensions;
using Application;
using Application.Common.Exceptions;
using Infrastructure;
using Infrastructure.Microservices.SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddApplication();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddControllers();
builder.Services.AddRouting();
builder.Services.AddProblemsExtension();
builder.Services.AddCorsPolicy(builder.Configuration, builder.Environment);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwaggerDocumentation();

// Only redirect to HTTPS in production environments
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowFrontend"); // Must come after UseRouting but before authentication
app.UseAuthentication(); // Must come before UseAuthorization
app.UseAuthorization();
app.UseExceptionHandler();
app.MapControllers();

// Identity API Endpoints
app.MapCustomIdentityApi();

// This creates the WebSocket endpoint at: ws://localhost:5000/hubs/veiling-klok
// Clients connect to this URL to establish real-time connection
app.MapHub<VeilingHub>("/hubs/veiling-klok").RequireCors("AllowFrontend");

// Map development endpoints (seeder, testing utilities, etc.)
app.MapDevelopmentEndpoints();

app.Run();


// Note: For running dotnet ( !!!! Run In the same order always !!!! )
//  dotnet ef database drop --project Infrastructure --startup-project API --force
//  dotnet ef migrations remove --project Infrastructure --startup-project API
//  dotnet ef migrations add InitialCreate --project Infrastructure --startup-project API
//  dotnet ef database update --project Infrastructure --startup-project API

// dotnet ef migrations remove RemoveTokensDB --project Infrastructure --startup-project API