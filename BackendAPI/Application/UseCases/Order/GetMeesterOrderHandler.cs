using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.Common.Models;
using Application.DTOs.Output;
using Application.Repositories;
using Domain.Exceptions;
using MediatR;

namespace Application.UseCases.Order;

public sealed record GetMeesterOrderCommand(Guid OrderId) : IRequest<OrderKwekerOutputDto>;

public sealed class GetMeesterOrderHandler
    : IRequestHandler<GetMeesterOrderCommand, OrderKwekerOutputDto>
{
    private readonly IOrderRepository _orderRepository;

    public GetMeesterOrderHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderKwekerOutputDto> Handle(
        GetMeesterOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        var result = await _orderRepository.GetOrderDetailsAsync(request.OrderId);

        if (result == null)
            throw OrderValidationException.OrderNotFound();

        return OrderMapper.KwekerOrders.ToOutputDto(result.Value);
    }
}
