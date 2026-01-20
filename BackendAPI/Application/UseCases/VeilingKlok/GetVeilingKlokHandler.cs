using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.Common.Models;
using Application.DTOs.Output;
using Application.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.VeilingKlok;

public sealed record GetVeilingKlokCommand(Guid Id) : IRequest<VeilingKlokOutputDto>;

public sealed class GetVeilingKlokHandler
    : IRequestHandler<GetVeilingKlokCommand, VeilingKlokOutputDto>
{
    private readonly IVeilingKlokRepository _veilingKlokRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetVeilingKlokHandler> _logger;

    public GetVeilingKlokHandler(
        IVeilingKlokRepository veilingKlokRepository,
        IProductRepository productRepository,
        ILogger<GetVeilingKlokHandler> logger
    )
    {
        _veilingKlokRepository = veilingKlokRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<VeilingKlokOutputDto> Handle(
        GetVeilingKlokCommand request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _veilingKlokRepository.GetByIdWithBidsCount(request.Id)
            ?? throw RepositoryException.NotFoundVeilingKlok();

        // Extract VeilingKlok and bid count
        var veilingKlok = result.VeilingKlok;
        var bidCount = result.BidCount;

        // Log VeilingKlok details for debugging
        _logger.LogInformation(
            "VeilingKlok retrieved - Id: {VeilingKlokId}, VeilingKlokProducts Count: {ProductsCount}, "
                + "LowestProductPrice: {LowestPrice}, HighestProductPrice: {HighestPrice}",
            veilingKlok.Id,
            veilingKlok.VeilingKlokProducts.Count,
            veilingKlok.LowestProductPrice,
            veilingKlok.HighestProductPrice
        );

        // Get products associated with the VeilingKlok (ordered by position)
        var productIds = veilingKlok.GetOrderedProductIds();

        var products =
            productIds.Count == 0
                ? new List<ProductOutputDto>()
                : (await _productRepository.GetAllByIdsWithKwekerInfoAsync(productIds))
                    .Select(r =>
                    {
                        var dto = ProductMapper.Minimal.ToOutputDto(r.Product, r.Kweker);
                        var vkp = veilingKlok.VeilingKlokProducts.FirstOrDefault(vp =>
                            vp.ProductId == r.Product.Id
                        );
                        if (vkp != null)
                            dto.AuctionedPrice = vkp.AuctionPrice;

                        return dto;
                    })
                    .ToList();

        // Calculate lowest and highest prices from the VeilingKlok entity
        var lowestPrice =
            veilingKlok.VeilingKlokProducts.Count > 0
                ? veilingKlok.LowestProductPrice
                : (decimal?)null;
        var highestPrice =
            veilingKlok.VeilingKlokProducts.Count > 0
                ? veilingKlok.HighestProductPrice
                : (decimal?)null;

        _logger.LogInformation(
            "Calculated prices - LowestPrice: {LowestPrice}, HighestPrice: {HighestPrice}",
            lowestPrice,
            highestPrice
        );

        var info = new VeilingKlokExtraInfo<ProductOutputDto>(
            bidCount,
            products,
            highestPrice,
            lowestPrice
        );
        return VeilingKlokMapper.Minimal.ToOutputDto(veilingKlok, info);
    }
}
