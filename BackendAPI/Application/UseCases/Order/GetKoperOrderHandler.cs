using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.Common.Models;
using Application.DTOs.Output;
using Application.Repositories;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.UseCases.Order;

public sealed record GetKoperOrderCommand(Guid OrderId, Guid KoperId) : IRequest<OrderKoperOutputDto>;

public sealed class GetKoperOrderHandler : IRequestHandler<GetKoperOrderCommand, OrderKoperOutputDto>
{
    private readonly IOrderRepository _orderRepository;

    public GetKoperOrderHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderKoperOutputDto> Handle(
        GetKoperOrderCommand request,
        CancellationToken cancellationToken
    )
    {
        var result = await _orderRepository.GetKoperOrderAsync(request.OrderId, request.KoperId);

        if (result == null)
            throw OrderValidationException.OrderNotFound();

        return OrderMapper.KoperOrders.ToOutputDto(result.Value);
    }
}