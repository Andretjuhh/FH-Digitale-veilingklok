using Application.DTOs.Output;
using Application.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Product;

public sealed record GetKwekerStatsCommand(Guid KwekerId) : IRequest<KwekerStatsOutputDto>;

public sealed class GetKwekerStatsHandler : IRequestHandler<GetKwekerStatsCommand, KwekerStatsOutputDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public GetKwekerStatsHandler(IProductRepository productRepository, IOrderRepository orderRepository)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    public async Task<KwekerStatsOutputDto> Handle(
        GetKwekerStatsCommand request,
        CancellationToken cancellationToken
    )
    {
        // Get all products for this kweker
        var (products, totalCount) = await _productRepository.GetAllWithFilterAsync(
            nameFilter: null,
            regionFilter: null,
            maxPrice: null,
            kwekerId: request.KwekerId,
            klokId: null,
            pageNumber: 1,
            pageSize: int.MaxValue
        );

        var productList = products.ToList();
        var totalProducts = totalCount;
        var activeAuctions = productList.Count(p => p.Product.IsBeingAuctioned);

        // Get all orders for this kweker's products
        var productIds = productList.Select(p => p.Product.Id).ToList();
        
        var (orders, _) = await _orderRepository.GetAllKwekerWithFilterAsync(
            ProductNameFilter: null,
            KoperNameFilter: null,
            statusFilter: null,
            beforeDate: null,
            afterDate: null,
            productId: null,
            kwekerId: request.KwekerId,
            pageNumber: 1,
            pageSize: int.MaxValue
        );

        var ordersList = orders.ToList();
        
        // Calculate total revenue from completed orders
        var completedOrders = ordersList
            .Where(o => o.Order.Status == OrderStatus.Processed || o.Order.Status == OrderStatus.Delivered)
            .ToList();

        decimal totalRevenue = 0;
        int ordersReceived = completedOrders.Count;

        foreach (var orderInfo in completedOrders)
        {
            var orderItems = orderInfo.Order.OrderItems
                .Where(oi => productIds.Contains(oi.ProductId))
                .ToList();

            foreach (var item in orderItems)
            {
                totalRevenue += item.PriceAtPurchase * item.Quantity;
            }
        }

        // Calculate monthly revenue for the last 12 months
        var monthlyRevenue = new List<MonthlyRevenueDto>();
        var today = DateTimeOffset.UtcNow;
        
        for (int i = 11; i >= 0; i--)
        {
            var targetDate = today.AddMonths(-i);
            var monthStart = new DateTimeOffset(targetDate.Year, targetDate.Month, 1, 0, 0, 0, TimeSpan.Zero);
            var monthEnd = monthStart.AddMonths(1);

            var monthRevenue = ordersList
                .Where(o => (o.Order.Status == OrderStatus.Processed || o.Order.Status == OrderStatus.Delivered)
                    && o.Order.CreatedAt >= monthStart
                    && o.Order.CreatedAt < monthEnd)
                .SelectMany(o => o.Order.OrderItems.Where(oi => productIds.Contains(oi.ProductId)))
                .Sum(item => item.PriceAtPurchase * item.Quantity);

            monthlyRevenue.Add(new MonthlyRevenueDto
            {
                Year = targetDate.Year,
                Month = targetDate.Month,
                MonthName = targetDate.ToString("MMM yyyy"),
                Revenue = monthRevenue
            });
        }

        return new KwekerStatsOutputDto
        {
            TotalProducts = totalProducts,
            ActiveAuctions = activeAuctions,
            TotalRevenue = totalRevenue,
            OrdersReceived = ordersReceived,
            MonthlyRevenue = monthlyRevenue
        };
    }
}
