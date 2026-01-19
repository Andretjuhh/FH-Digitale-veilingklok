using Application.Common.Exceptions;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace API.Extensions;

public static class IdentityExtension
{
    public static IEndpointRouteBuilder MapCustomIdentityApi(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/api/account")
            .MapIdentityApi<Account>()
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
                        if (
                            actualResult
                            is Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult problem
                        )
                            if (problem.StatusCode == 401)
                            {
                                if (problem.ProblemDetails.Detail == "LockedOut")
                                {
                                    string? email = null;

                                    if (context.HttpContext.Request.HasFormContentType)
                                    {
                                        email = context
                                            .HttpContext.Request.Form["email"]
                                            .ToString();
                                    }
                                    else if (context.Arguments.FirstOrDefault() is { } loginRequest)
                                    {
                                        // Try to get Email property via reflection (for JSON requests)
                                        email = loginRequest
                                            .GetType()
                                            .GetProperty("Email")
                                            ?.GetValue(loginRequest)
                                            ?.ToString();
                                    }

                                    if (string.IsNullOrEmpty(email))
                                    {
                                        throw CustomException.AccountLocked();
                                    }

                                    // We need to resolve UserManager to check if it's actually deleted
                                    var userManager =
                                        context.HttpContext.RequestServices.GetRequiredService<
                                            UserManager<Account>
                                        >();
                                    var user = await userManager.FindByEmailAsync(email);

                                    if (user != null && user.DeletedAt.HasValue)
                                    {
                                        throw CustomException.AccountDeactivated();
                                    }

                                    throw CustomException.AccountLocked();
                                }
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

        return app;
    }
}
