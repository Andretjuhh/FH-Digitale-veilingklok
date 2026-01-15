using Application.DTOs.Output;
using Application.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.VeilingKlok;

public record GetMeesterStatsCommand(Guid MeesterId) : IRequest<MeesterStatsOutputDto>;

public class GetMeesterStatsHandler : IRequestHandler<GetMeesterStatsCommand, MeesterStatsOutputDto>
{
    private readonly IVeilingKlokRepository _veilingKlokRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public GetMeesterStatsHandler(
        IVeilingKlokRepository veilingKlokRepository,
        IProductRepository productRepository,
        IOrderRepository orderRepository
    )
    {
        _veilingKlokRepository = veilingKlokRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    public async Task<MeesterStatsOutputDto> Handle(
        GetMeesterStatsCommand request,
        CancellationToken cancellationToken
    )
    {
        // Get all veilingklokken for this meester
        var (klokken, totalKlokken) = await _veilingKlokRepository.GetAllWithFilterAndBidsAsync(
            statusFilter: null,
            region: null,
            scheduledAfter: null,
            scheduledBefore: null,
            startedAfter: null,
            startedBefore: null,
            endedAfter: null,
            endedBefore: null,
            meesterId: request.MeesterId,
            pageNumber: 1,
            pageSize: int.MaxValue
        );

        var klokkenList = klokken.ToList();

        // Count active veilingklokken (Started or Paused)
        var activeKlokken = klokkenList.Count(k => 
            k.VeilingKlok.Status == VeilingKlokStatus.Started || k.VeilingKlok.Status == VeilingKlokStatus.Paused
        );

        // Get all products across all veilingklokken
        var klokIds = klokkenList.Select(k => k.VeilingKlok.Id).ToList();
        int totalProducts = 0;
        int totalOrders = 0;

        foreach (var klokId in klokIds)
        {
            // Get products for this klok
            var (products, productCount) = await _productRepository.GetAllWithFilterAsync(
                nameFilter: null,
                regionFilter: null,
                maxPrice: null,
                kwekerId: null,
                klokId: klokId,
                pageNumber: 1,
                pageSize: int.MaxValue
            );

            totalProducts += productCount;

            // Get orders for this klok
            var (orders, orderCount) = await _orderRepository.GetAllWithFilterAsync(
                statusFilter: null,
                beforeDate: null,
                afterDate: null,
                productId: null,
                koperId: null,
                klokId: klokId,
                pageNumber: 1,
                pageSize: int.MaxValue
            );

            totalOrders += orderCount;
        }

        return new MeesterStatsOutputDto
        {
            TotalVeilingKlokken = totalKlokken,
            ActiveVeilingKlokken = activeKlokken,
            TotalProducts = totalProducts,
            TotalOrders = totalOrders
        };
    }
}
