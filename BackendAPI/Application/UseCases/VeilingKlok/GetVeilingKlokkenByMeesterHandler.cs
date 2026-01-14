using Application.Common.Mappers;
using Application.Common.Models;
using Application.DTOs.Output;
using Application.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.VeilingKlok;

public sealed record GetVeilingKlokkenByMeesterCommand(Guid MeesterId)
    : IRequest<List<VeilingKlokOutputDto>>;

public sealed class GetVeilingKlokkenByMeesterHandler
    : IRequestHandler<GetVeilingKlokkenByMeesterCommand, List<VeilingKlokOutputDto>>
{
    private readonly IVeilingKlokRepository _veilingKlokRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetVeilingKlokkenByMeesterHandler> _logger;

    public GetVeilingKlokkenByMeesterHandler(
        IVeilingKlokRepository veilingKlokRepository,
        IProductRepository productRepository,
        ILogger<GetVeilingKlokkenByMeesterHandler> logger
    )
    {
        _veilingKlokRepository = veilingKlokRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<List<VeilingKlokOutputDto>> Handle(
        GetVeilingKlokkenByMeesterCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var klokken = await _veilingKlokRepository.GetAllByMeesterIdWithBidsCountAsync(
                request.MeesterId
            );

            var results = new List<VeilingKlokOutputDto>();

            foreach (var item in klokken)
            {
                var veilingKlok = item.VeilingKlok;
                var bidCount = item.BidCount;

                var products = (await _productRepository.GetAllByVeilingKlokIdWithKwekerInfoAsync(veilingKlok.Id))
                    .Select(r => ProductMapper.Minimal.ToOutputDto(r.Product, r.Kweker))
                    .ToList();
                if (products.Count == 0)
                {
                    products = (await _productRepository.GetAllByOrderItemsVeilingKlokIdWithKwekerInfoAsync(veilingKlok.Id))
                        .Select(r => ProductMapper.Minimal.ToOutputDto(r.Product, r.Kweker))
                        .ToList();
                }

                var info = new VeilingKlokExtraInfo<ProductOutputDto>(
                    bidCount,
                    products,
                    veilingKlok.HighestPrice,
                    veilingKlok.LowestPrice
                );

                results.Add(VeilingKlokMapper.Minimal.ToOutputDto(veilingKlok, info));
            }

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to load veilingklokken for veilingmeester {MeesterId}",
                request.MeesterId
            );
            throw;
        }
    }
}
