using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Output;
using Application.Repositories;
using MediatR;

namespace Application.UseCases.Order;

public sealed record GetOrderQuery(Guid OrderId) : IRequest<OrderOutputDto>;

public sealed class GetOrderHandler : IRequestHandler<GetOrderQuery, OrderOutputDto>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderOutputDto> Handle(
        GetOrderQuery request,
        CancellationToken cancellationToken
    )
    {
        var order =
            await _orderRepository.GetByIdAsync(request.OrderId)
            ?? throw RepositoryException.NotFoundOrder();

        return OrderMapper.ToOutputDto(order);
    }
}