using Microsoft.AspNetCore.Mvc;

namespace API.Models;

[Serializable]
public class HttpError : IActionResult
{
    public int StatusCode { get; private set; }
    public string Name { get; private set; }
    public string Message { get; private set; }
    public string? Details { get; private set; }
    public string? RequestId { get; private set; }
    public string? TraceId { get; private set; }
    public DateTimeOffset Timestamp { get; private set; }

    private HttpError()
    {
        Name = string.Empty;
        Message = string.Empty;
        Timestamp = DateTimeOffset.UtcNow;
    }

    public HttpError(int code, string name, string message)
    {
        Name = name;
        StatusCode = code;
        Message = message;
        Timestamp = DateTimeOffset.UtcNow;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var response = new
        {
            StatusCode = StatusCode,
            Name = Name,
            Message = Message,
            Details = Details,
            RequestId = RequestId,
            TraceId = TraceId,
            Timestamp = Timestamp,
        };

        context.HttpContext.Response.StatusCode = StatusCode;
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(response);
    }

    // Private builder methods
    private HttpError WithStatusCode(int statusCode)
    {
        StatusCode = statusCode;
        return this;
    }

    private HttpError WithName(string name)
    {
        Name = name;
        return this;
    }

    private HttpError WithMessage(string message)
    {
        Message = message;
        return this;
    }

    // Public methods
    public HttpError WithDetails(string details)
    {
        Details = details;
        return this;
    }

    public HttpError WithRequestId(string requestId)
    {
        RequestId = requestId;
        return this;
    }

    public HttpError WithTraceId(string? traceId)
    {
        TraceId = traceId;
        return this;
    }

    // Static factory methods for common error responses
    public static HttpError BadRequest(string message)
    {
        return new HttpError()
            .WithStatusCode(StatusCodes.Status400BadRequest)
            .WithName("Bad Request")
            .WithMessage(message);
    }

    public static HttpError Unauthorized(string message = "Unauthorized access")
    {
        return new HttpError()
            .WithStatusCode(StatusCodes.Status401Unauthorized)
            .WithName("Unauthorized")
            .WithMessage(message);
    }

    public static HttpError Forbidden(string message = "Access forbidden")
    {
        return new HttpError()
            .WithStatusCode(StatusCodes.Status403Forbidden)
            .WithName("Forbidden")
            .WithMessage(message);
    }

    public static HttpError NotFound(string message)
    {
        return new HttpError()
            .WithStatusCode(StatusCodes.Status404NotFound)
            .WithName("Not Found")
            .WithMessage(message);
    }

    public static HttpError Conflict(string message)
    {
        return new HttpError()
            .WithStatusCode(StatusCodes.Status409Conflict)
            .WithName("Conflict")
            .WithMessage(message);
    }

    public static HttpError InternalServerError(
        string message = "An internal server error occurred"
    )
    {
        return new HttpError()
            .WithStatusCode(StatusCodes.Status500InternalServerError)
            .WithName("Internal Server Error")
            .WithMessage(message);
    }

    public static HttpError Custom(int statusCode, string name, string message)
    {
        return new HttpError().WithStatusCode(statusCode).WithName(name).WithMessage(message);
    }
}
