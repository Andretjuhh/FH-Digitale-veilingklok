using Application.DTOs.Output;
using Application.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Product;

public record GetKwekerProductStatsCommand(Guid KwekerId) : IRequest<KwekerProductStatsOutputDto>;

public class GetKwekerProductStatsHandler : IRequestHandler<GetKwekerProductStatsCommand, KwekerProductStatsOutputDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IVeilingKlokRepository _veilingKlokRepository;

    public GetKwekerProductStatsHandler(IProductRepository productRepository, IVeilingKlokRepository veilingKlokRepository)
    {
        _productRepository = productRepository;
        _veilingKlokRepository = veilingKlokRepository;
    }

    public async Task<KwekerProductStatsOutputDto> Handle(GetKwekerProductStatsCommand request, CancellationToken cancellationToken)
    {
        // Get all products for this kweker
        var (productList, totalCount) = await _productRepository.GetAllWithFilterAsync(
            null, null, null, request.KwekerId, null, 1, int.MaxValue
        );

        // Calculate inventory stock (sum of all product quantities)
        var inventoryStock = productList.Sum(p => p.Product.Stock);

        // Get active auctions - veilingkloks that are started
        var activeKloks = await _veilingKlokRepository.GetAllByStatusAsync(VeilingKlokStatus.Started, cancellationToken);
        
        // For now, count all started kloks as potential active auctions for any kweker
        // (A more accurate count would require checking VeilingKlokProduct relationship)
        var activeAuctions = activeKloks.Count();

        return new KwekerProductStatsOutputDto
        {
            TotalProducts = totalCount,
            InventoryStock = inventoryStock,
            ActiveAuctions = activeAuctions
        };
    }
}
