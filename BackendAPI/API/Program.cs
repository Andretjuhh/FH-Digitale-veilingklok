using API.Extensions;
using Application;
using Application.Common.Exceptions;
using Infrastructure;
using Infrastructure.Microservices.SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.AddControllers();

builder.Services.AddRouting();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                // Local development
                "http://localhost:3000",

                // Azure frontend (DIT WAS DE MISSENDE)
                "https://veilingklok-frontendklas1groep2-hvcmg3eua4fhgqft.germanywestcentral-01.azurewebsites.net"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


builder.Services.AddSwaggerDocumentation();
builder.Services.AddProblemsExtension();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

#endregion

var app = builder.Build();

#region Middleware pipeline (VOLGORDE IS KRITISCH)

// Swagger (ook in Azure beschikbaar)
app.UseSwaggerDocumentation();

// HTTPS alleen afdwingen buiten development
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Static files (wwwroot)
app.UseDefaultFiles();   // index.html
app.UseStaticFiles();    // css/js/images

app.UseRouting();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();

#endregion

#region Endpoints

app.MapControllers();

// Identity API
app.MapGroup("/api/account")
    .MapIdentityApi<Domain.Entities.Account>()
    .RequireCors("AllowFrontend")
    .AddEndpointFilter(
        async (context, next) =>
        {
            var result = await next(context);

            if (
                context.HttpContext.Request.Path.Value?.EndsWith(
                    "/login",
                    StringComparison.OrdinalIgnoreCase
                ) == true
            )
            {
                var resultType = result?.GetType();
                var resultProperty = resultType?.GetProperty("Result");
                var actualResult = resultProperty?.GetValue(result);

                if (actualResult is Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult problem)
                {
                    if (problem.StatusCode == 401)
                    {
                        if (problem.ProblemDetails.Detail == "LockedOut")
                            throw CustomException.AccountLocked();
                        else
                            throw CustomException.InvalidCredentials();
                    }
                }

                if (actualResult is IStatusCodeHttpResult { StatusCode: 401 })
                    throw CustomException.InvalidCredentials();
            }

            return result;
        }
    );

// SignalR
app.MapHub<VeilingHub>("/hubs/veiling-klok")
   .RequireCors("AllowFrontend");

// Development-only endpoints
if (app.Environment.IsDevelopment())
{
    app.MapDevelopmentEndpoints();
}

#endregion

app.Run();

// Note: For running dotnet ( !!!! Run In the same order always !!!! )
//  dotnet ef database drop --project Infrastructure --startup-project API --force
//  dotnet ef migrations remove --project Infrastructure --startup-project API
//  dotnet ef migrations add InitialCreate --project Infrastructure --startup-project API
//  dotnet ef database update --project Infrastructure --startup-project API
