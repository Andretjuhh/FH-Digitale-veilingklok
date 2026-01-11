using Application.DTOs.Output;
using Application.Repositories;
using Application.Common.Mappers;
using Application.Common.Models;
using Domain.Exceptions;
using MediatR;

namespace Application.UseCases.Order;

public sealed record GetKwekerOrderCommand(Guid OrderId, Guid KwekerId) : IRequest<OrderKwekerOutput>;

public class GetKwekerOrderHandler : IRequestHandler<GetKwekerOrderCommand, OrderKwekerOutput>
{
    private readonly IOrderRepository _orderRepository;

    public GetKwekerOrderHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderKwekerOutput> Handle(
        GetKwekerOrderCommand request,
        CancellationToken cancellationToken)
    {
        // Get the specific order for the kweker
        var orderData = await _orderRepository.GetKwekerOrderAsync(
            request.OrderId,
            request.KwekerId);

        // Handle null result with custom exception
        if (orderData == null)
            throw OrderValidationException.OrderNotFound();

        // Map the result using the optimized mapper
        var result = OrderMapper.KwekerOrders.ToOutputDto(orderData.Value);

        return result;
    }
}