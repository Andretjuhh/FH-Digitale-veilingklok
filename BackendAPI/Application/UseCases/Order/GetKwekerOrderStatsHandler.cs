using Application.DTOs.Output;
using Application.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Order;

public record GetKwekerOrderStatsCommand(Guid KwekerId) : IRequest<KwekerOrderStatsOutputDto>;

public class GetKwekerOrderStatsHandler : IRequestHandler<GetKwekerOrderStatsCommand, KwekerOrderStatsOutputDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public GetKwekerOrderStatsHandler(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<KwekerOrderStatsOutputDto> Handle(GetKwekerOrderStatsCommand request, CancellationToken cancellationToken)
    {
        // Get all orders for this kweker's products
        var (allOrders, totalCount) = await _orderRepository.GetAllKwekerWithFilterAsync(
            null, null, null, null, null, null, request.KwekerId, 1, int.MaxValue
        );

        // Count orders by status
        var pendingOrders = allOrders.Count(o => o.Order.Status == OrderStatus.Open || o.Order.Status == OrderStatus.Processing);
        var completedOrders = allOrders.Count(o => o.Order.Status == OrderStatus.Delivered || o.Order.Status == OrderStatus.Processed);
        var canceledOrders = allOrders.Count(o => o.Order.Status == OrderStatus.Cancelled);

        return new KwekerOrderStatsOutputDto
        {
            TotalOrders = totalCount,
            PendingOrders = pendingOrders,
            CompletedOrders = completedOrders,
            CanceledOrders = canceledOrders
        };
    }
}
