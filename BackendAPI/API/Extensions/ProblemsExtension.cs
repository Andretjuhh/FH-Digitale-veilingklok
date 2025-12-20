using API.Models;
using Application.Common.Exceptions;
using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace API.Extensions;

public static class ProblemsExtension
{
    public static IServiceCollection AddProblemsExtension(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
                context.ProblemDetails.Extensions.TryAdd(
                    "requestId",
                    context.HttpContext.TraceIdentifier
                );
                context.ProblemDetails.Extensions.TryAdd("timestamp", DateTime.UtcNow);
            };
        });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context
                    .ModelState.Where(e => e.Value?.Errors.Count > 0)
                    .SelectMany(e => e.Value!.Errors.Select(x => x.ErrorMessage))
                    .FirstOrDefault();

                var errorMessage = errors ?? "Invalid request data";
                return HttpError.BadRequest(errorMessage);
            };
        });

        services.AddExceptionHandler<ProblemExceptionHandler>();

        return services;
    }

    internal class ProblemExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;

        public ProblemExceptionHandler(IProblemDetailsService problemDetailsService)
        {
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            HttpError errorResponse;

            switch (exception)
            {
                case ProcessException processException:
                    errorResponse = HttpError.Custom(
                        processException.StatusCode,
                        processException.MessageCode,
                        processException.Message ?? "Process error occurred"
                    );
                    break;

                case DomainException domainException:
                    errorResponse = HttpError.BadRequest(
                        domainException.Message ?? domainException.ErrorCode
                    );
                    break;

                default:
                    errorResponse = HttpError.InternalServerError("An unexpected error occurred");
                    break;
            }

            var activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;
            errorResponse.WithRequestId(httpContext.TraceIdentifier).WithTraceId(activity?.Id);

            httpContext.Response.StatusCode = errorResponse.StatusCode;
            await errorResponse.ExecuteResultAsync(new ActionContext { HttpContext = httpContext });
            return true;
        }
    }
}