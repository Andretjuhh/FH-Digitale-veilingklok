using API.Models;
using Application.Common.Exceptions;
using Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

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
        private readonly ILogger<ProblemExceptionHandler> _logger;

        public ProblemExceptionHandler(
            IProblemDetailsService problemDetailsService,
            ILogger<ProblemExceptionHandler> logger)
        {
            _problemDetailsService = problemDetailsService;
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken
        )
        {
            HttpError errorResponse;

            // Pattern matching automatically catches all derived classes
            switch (exception)
            {
                case ProcessException processException:
                    // This catches CustomException, NotFoundException, and any other ProcessException derived class
                    errorResponse = HttpError.Custom(
                        processException.StatusCode,
                        processException.MessageCode,
                        processException.MessageCode
                    );
                    break;

                case DomainException domainException:
                    // This catches all DomainException derived classes
                    errorResponse = HttpError.BadRequest(domainException.ErrorCode);
                    break;

                default:
                    _logger.LogError(exception, "Unhandled exception occurred");
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