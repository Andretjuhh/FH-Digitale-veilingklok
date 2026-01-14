using System.Collections.Generic;
using MediatR;

namespace Test.Fakes;

public class FakeMediator : IMediator
{
    private readonly Dictionary<Type, object?> _responses = new();

    public object? LastRequest { get; private set; }

    public void RegisterResponse<TRequest, TResponse>(TResponse response)
        where TRequest : IRequest<TResponse>
    {
        _responses[typeof(TRequest)] = response;
    }

    // For commands that do not return a value (IRequest)
    public Task Send<TRequest>(
        TRequest request,
        CancellationToken cancellationToken = default
    ) where TRequest : IRequest
    {
        LastRequest = request!;
        return Task.CompletedTask;
    }

    public Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default
    )
    {
        LastRequest = request;

        var requestType = request.GetType();

        if (_responses.TryGetValue(requestType, out var value))
        {
            return Task.FromResult((TResponse)value!);
        }

        // If no response is registered, return default. This is fine for
        // commands where the controller ignores the MediatR result.
        return Task.FromResult(default(TResponse)!);
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Use generic Send<TResponse> in tests.");
    }

    public Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task Publish<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default
    ) where TNotification : INotification
    {
        return Task.CompletedTask;
    }

    // Streaming APIs are not used in these tests; throw if accidentally called.
    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
        IStreamRequest<TResponse> request,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotSupportedException("Streaming is not supported in FakeMediator.");
    }

    public IAsyncEnumerable<object?> CreateStream(
        object request,
        CancellationToken cancellationToken = default
    )
    {
        throw new NotSupportedException("Streaming is not supported in FakeMediator.");
    }
}
