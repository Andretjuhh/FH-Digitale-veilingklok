using Application.DTOs.Output;
using Application.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Order;

public record GetKoperStatsCommand(Guid KoperId) : IRequest<KoperStatsOutputDto>;

public class GetKoperStatsHandler : IRequestHandler<GetKoperStatsCommand, KoperStatsOutputDto>
{
    private readonly IOrderRepository _orderRepository;

    public GetKoperStatsHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<KoperStatsOutputDto> Handle(
        GetKoperStatsCommand request,
        CancellationToken cancellationToken
    )
    {
        // Get all orders for this koper
        var (items, totalCount) = await _orderRepository.GetAllWithFilterAsync(
            statusFilter: null,
            beforeDate: null,
            afterDate: null,
            productId: null,
            koperId: request.KoperId,
            klokId: null,
            pageNumber: 1,
            pageSize: int.MaxValue
        );

        if (totalCount == 0)
        {
            return new KoperStatsOutputDto
            {
                TotalOrders = 0,
                PendingOrders = 0,
                CompletedOrders = 0,
                CanceledOrders = 0,
            };
        }

        var ordersList = items.Select(x => x.order).ToList();

        // Count orders by status
        var pendingOrders = ordersList.Count(o =>
            o.Status == OrderStatus.Open || o.Status == OrderStatus.Processing
        );
        var completedOrders = ordersList.Count(o =>
            o.Status == OrderStatus.Delivered || o.Status == OrderStatus.Processed
        );
        var canceledOrders = ordersList.Count(o => o.Status == OrderStatus.Cancelled);

        return new KoperStatsOutputDto
        {
            TotalOrders = totalCount,
            PendingOrders = pendingOrders,
            CompletedOrders = completedOrders,
            CanceledOrders = canceledOrders,
        };
    }
}
