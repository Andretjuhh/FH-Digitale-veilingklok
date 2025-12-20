using System.ComponentModel.DataAnnotations;

namespace Application.Common.Exceptions;

public abstract class ProcessException : Exception
{
    [Range(100, 599)] public int StatusCode { get; private init; }

    [MaxLength(10)] public string MessageCode { get; private init; }

    protected ProcessException(int statusCode, string messageCode, string? message)
        : base(message)
    {
        StatusCode = statusCode;
        MessageCode = messageCode;
    }

    protected ProcessException(int statusCode, string messageCode)
    {
        StatusCode = statusCode;
        MessageCode = messageCode;
    }
}