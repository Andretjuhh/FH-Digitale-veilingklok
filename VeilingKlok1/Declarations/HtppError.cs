using Microsoft.AspNetCore.Mvc;

namespace VeilingKlokApp.Declarations
{
    public class HtppError : IActionResult
    {
        public int StatusCode { get; private set; }
        public string Name { get; private set; }
        public string Message { get; private set; }
        public string? Details { get; private set; }
        public string? RequestId { get; private set; }
        public string? TraceId { get; private set; }
        public DateTime Timestamp { get; private set; }

        private HtppError()
        {
            Name = string.Empty;
            Message = string.Empty;
            Timestamp = DateTime.UtcNow;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var response = new
            {
                StatusCode = this.StatusCode,
                Name = this.Name,
                Message = this.Message,
                Details = this.Details,
                RequestId = this.RequestId,
                TraceId = this.TraceId,
                Timestamp = this.Timestamp,
            };

            context.HttpContext.Response.StatusCode = this.StatusCode;
            context.HttpContext.Response.ContentType = "application/json";
            await context.HttpContext.Response.WriteAsJsonAsync(response);
        }

        // Private builder methods
        private HtppError WithStatusCode(int statusCode)
        {
            StatusCode = statusCode;
            return this;
        }

        private HtppError WithName(string name)
        {
            Name = name;
            return this;
        }

        private HtppError WithMessage(string message)
        {
            Message = message;
            return this;
        }

        // Public method to add details
        public HtppError WithDetails(string details)
        {
            Details = details;
            return this;
        }

        // Static factory methods for common error responses
        public static HtppError BadRequest(string message) =>
            new HtppError()
                .WithStatusCode(StatusCodes.Status400BadRequest)
                .WithName("Bad Request")
                .WithMessage(message);

        public static HtppError Unauthorized(string message = "Unauthorized access") =>
            new HtppError()
                .WithStatusCode(StatusCodes.Status401Unauthorized)
                .WithName("Unauthorized")
                .WithMessage(message);

        public static HtppError Forbidden(string message = "Access forbidden") =>
            new HtppError()
                .WithStatusCode(StatusCodes.Status403Forbidden)
                .WithName("Forbidden")
                .WithMessage(message);

        public static HtppError NotFound(string message) =>
            new HtppError()
                .WithStatusCode(StatusCodes.Status404NotFound)
                .WithName("Not Found")
                .WithMessage(message);

        public static HtppError Conflict(string message) =>
            new HtppError()
                .WithStatusCode(StatusCodes.Status409Conflict)
                .WithName("Conflict")
                .WithMessage(message);

        public static HtppError InternalServerError(
            string message = "An internal server error occurred"
        ) =>
            new HtppError()
                .WithStatusCode(StatusCodes.Status500InternalServerError)
                .WithName("Internal Server Error")
                .WithMessage(message);

        public static HtppError Custom(int statusCode, string name, string message) =>
            new HtppError().WithStatusCode(statusCode).WithName(name).WithMessage(message);
    }
}
