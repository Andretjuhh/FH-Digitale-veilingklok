using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace API.Models;

[Serializable]
public class HttpSuccess<T> : IActionResult
{
    public int StatusCode { get; private set; }
    public string Message { get; private set; }
    public T? Data { get; private set; }
    public DateTime Timestamp { get; private set; }

    private HttpSuccess()
    {
        Message = string.Empty;
        Timestamp = DateTime.UtcNow;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var response = new
        {
            StatusCode = StatusCode,
            Message = Message,
            Data = Data,
            Timestamp = Timestamp,
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() },
        };

        context.HttpContext.Response.StatusCode = StatusCode;
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(response, options);
    }

    // Private builder methods
    private HttpSuccess<T> WithStatusCode(int statusCode)
    {
        StatusCode = statusCode;
        return this;
    }

    private HttpSuccess<T> WithMessage(string message)
    {
        Message = message;
        return this;
    }

    private HttpSuccess<T> WithData(T data)
    {
        Data = data;
        return this;
    }

    // Static factory methods for common success responses
    public static HttpSuccess<T> Ok(T data, string message = "Success")
    {
        return new HttpSuccess<T>()
            .WithStatusCode(StatusCodes.Status200OK)
            .WithMessage(message)
            .WithData(data);
    }

    public static HttpSuccess<T> Created(T data, string message = "Resource created successfully")
    {
        return new HttpSuccess<T>()
            .WithStatusCode(StatusCodes.Status201Created)
            .WithMessage(message)
            .WithData(data);
    }

    public static HttpSuccess<T> Accepted(
        T data,
        string message = "Request accepted for processing"
    )
    {
        return new HttpSuccess<T>()
            .WithStatusCode(StatusCodes.Status202Accepted)
            .WithMessage(message)
            .WithData(data);
    }

    public static HttpSuccess<object> NoContent(string message = "No content")
    {
        return new HttpSuccess<object>()
            .WithStatusCode(StatusCodes.Status204NoContent)
            .WithMessage(message);
    }

    public static HttpSuccess<T> Custom(int statusCode, T data, string message)
    {
        return new HttpSuccess<T>().WithStatusCode(statusCode).WithMessage(message).WithData(data);
    }
}
