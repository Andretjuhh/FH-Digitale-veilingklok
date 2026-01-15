using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.Common.Models;
using Application.DTOs.Output;
using Application.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.VeilingKlok;

public sealed record GetVeilingKlokDetailsCommand(Guid Id) : IRequest<VeilingKlokDetailsOutputDto>;

public sealed class GetVeilingDetailsKlokHandler
    : IRequestHandler<GetVeilingKlokDetailsCommand, VeilingKlokDetailsOutputDto>
{
    private readonly IVeilingKlokRepository _veilingKlokRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetVeilingDetailsKlokHandler> _logger;

    public GetVeilingDetailsKlokHandler(
        IVeilingKlokRepository veilingKlokRepository,
        IProductRepository productRepository,
        ILogger<GetVeilingDetailsKlokHandler> logger
    )
    {
        _veilingKlokRepository = veilingKlokRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<VeilingKlokDetailsOutputDto> Handle(
        GetVeilingKlokDetailsCommand request,
        CancellationToken cancellationToken
    )
    {
        // Fetch veiling klok with bid count
        var result =
            await _veilingKlokRepository.GetByIdWithBidsCount(request.Id)
            ?? throw RepositoryException.NotFoundVeilingKlok();

        // Extract veiling klok and bid count
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

        // Fetch associated products (ordered by position)
        var productIds = veilingKlok.GetOrderedProductIds();

        var products =
            productIds.Count == 0
                ? new List<ProductDetailsOutputDto>()
                : (await _productRepository.GetAllByIdsWithKwekerInfoAsync(productIds))
                    .Select(r =>
                    {
                        var dto = ProductMapper.ToOutputDto(r.Product, r.Kweker);
                        var vkp = veilingKlok.VeilingKlokProducts.FirstOrDefault(vp =>
                            vp.ProductId == r.Product.Id
                        );
                        if (vkp != null)
                        {
                            dto.AuctionPrice = vkp.AuctionPrice;
                        }

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

        // Create extra info and map to output DTO
        var extraInfo = new VeilingKlokExtraInfo<ProductDetailsOutputDto>(
            bidCount,
            products,
            highestPrice,
            lowestPrice
        );
        return VeilingKlokMapper.ToOutputDto(veilingKlok, extraInfo);
    }
}
