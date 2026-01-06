using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Output;
using Application.Repositories;
using MediatR;

namespace Application.UseCases.Order;

public sealed record GetOrderCommand(Guid OrderId, Guid? KoperId) : IRequest<OrderDetailsOutputDto>;

public sealed class GetOrderHandler : IRequestHandler<GetOrderCommand, OrderDetailsOutputDto>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderDetailsOutputDto> Handle(
        GetOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        var (order, infos) = (request.KoperId.HasValue
                                 ? await _orderRepository.GetWithProductsByIdAsync(request.OrderId,
                                     request.KoperId.Value)
                                 : await _orderRepository.GetWithProductsByIdAsync(request.OrderId)
                             )
                             ?? throw RepositoryException.NotFoundOrder();

        return OrderMapper.ExtraDetails.ToOutputDto(order, infos);
    }
}