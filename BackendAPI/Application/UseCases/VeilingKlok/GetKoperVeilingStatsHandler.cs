using Application.DTOs.Output;
using Application.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.VeilingKlok;

public record GetKoperVeilingStatsCommand(Guid KoperId) : IRequest<KoperVeilingStatsOutputDto>;

public class GetKoperVeilingStatsHandler : IRequestHandler<GetKoperVeilingStatsCommand, KoperVeilingStatsOutputDto>
{
    private readonly IVeilingKlokRepository _veilingKlokRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public GetKoperVeilingStatsHandler(
        IVeilingKlokRepository veilingKlokRepository,
        IProductRepository productRepository,
        IOrderRepository orderRepository
    )
    {
        _veilingKlokRepository = veilingKlokRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    public async Task<KoperVeilingStatsOutputDto> Handle(
        GetKoperVeilingStatsCommand request,
        CancellationToken cancellationToken
    )
    {
        // Get all active veilingklokken (Started or Paused)
        var activeKlokken = await _veilingKlokRepository.GetAllByStatusAsync(VeilingKlokStatus.Started, cancellationToken);
        var pausedKlokken = await _veilingKlokRepository.GetAllByStatusAsync(VeilingKlokStatus.Paused, cancellationToken);
        var activeAuctions = activeKlokken.Count() + pausedKlokken.Count();

        // Get scheduled veilingklokken
        var scheduledKlokken = await _veilingKlokRepository.GetAllByStatusAsync(VeilingKlokStatus.Scheduled, cancellationToken);
        var scheduledAuctions = scheduledKlokken.Count();

        // Count available products in active auctions
        int availableProducts = 0;
        var allActiveKlokken = activeKlokken.Concat(pausedKlokken);
        foreach (var klok in allActiveKlokken)
        {
            var (products, count) = await _productRepository.GetAllWithFilterAsync(
                nameFilter: null,
                regionFilter: null,
                maxPrice: null,
                kwekerId: null,
                klokId: klok.Id,
                pageNumber: 1,
                pageSize: int.MaxValue
            );
            availableProducts += count;
        }

        // Get purchases for this koper
        var (orders, totalOrders) = await _orderRepository.GetAllWithFilterAsync(
            statusFilter: null,
            beforeDate: null,
            afterDate: null,
            productId: null,
            koperId: request.KoperId,
            klokId: null,
            pageNumber: 1,
            pageSize: int.MaxValue
        );

        return new KoperVeilingStatsOutputDto
        {
            ActiveAuctions = activeAuctions,
            ScheduledAuctions = scheduledAuctions,
            AvailableProducts = availableProducts,
            YourPurchases = totalOrders
        };
    }
}
