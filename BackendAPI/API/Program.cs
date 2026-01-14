using API.Extensions;
using Application;
using Infrastructure;
using Infrastructure.Microservices.SignalR.Hubs;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.WebHost.ConfigureKestrel(options =>
{
    var port = Environment.GetEnvironmentVariable("PORT");
    if (!string.IsNullOrEmpty(port))
        options.ListenAnyIP(int.Parse(port));
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddControllers();
builder.Services.AddProblemsExtension();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

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
app.MapHub<VeilingHub>("/hubs/veiling-klok");

// ONLY IN DEV
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();


// // Note: For running dotnet ( !!!! Run In the same order always !!!! )
// //  dotnet ef database drop --project Infrastructure --startup-project API --force
// //  dotnet ef migrations remove --project Infrastructure --startup-project API
// //  dotnet ef migrations add InitialCreate --project Infrastructure --startup-project API
// //  dotnet ef database update --project Infrastructure --startup-project API
