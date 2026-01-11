using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Output;
using Application.Repositories;
using MediatR;
using Application.Common.Models;

namespace Application.UseCases.VeilingKlok;

public sealed record GetVeilingKlokCommand(Guid Id) : IRequest<VeilingKlokOutputDto>;

public sealed class GetVeilingKlokHandler
    : IRequestHandler<GetVeilingKlokCommand, VeilingKlokOutputDto>
{
    private readonly IVeilingKlokRepository _veilingKlokRepository;
    private readonly IProductRepository _productRepository;

    public GetVeilingKlokHandler(IVeilingKlokRepository veilingKlokRepository, IProductRepository productRepository)
    {
        _veilingKlokRepository = veilingKlokRepository;
        _productRepository = productRepository;
    }

    public async Task<VeilingKlokOutputDto> Handle(
        GetVeilingKlokCommand request,
        CancellationToken cancellationToken
    )
    {
        var result = await _veilingKlokRepository.GetByIdWithBidsCount(request.Id) ??
                     throw RepositoryException.NotFoundVeilingKlok();

        // Extract VeilingKlok and bid count
        var veilingKlok = result.VeilingKlok;
        var bidCount = result.BidCount;

        // Get products associated with the VeilingKlok
        var products = (await _productRepository.GetAllByVeilingKlokIdWithKwekerInfoAsync(veilingKlok.Id))
            .Select(r => ProductMapper.Minimal.ToOutputDto(r.Product, r.Kweker)).ToList();
        if (products.Count == 0)
        {
            products = (await _productRepository.GetAllByOrderItemsVeilingKlokIdWithKwekerInfoAsync(veilingKlok.Id))
                .Select(r => ProductMapper.Minimal.ToOutputDto(r.Product, r.Kweker)).ToList();
        }

        var info = new VeilingKlokExtraInfo<ProductOutputDto>(
            bidCount,
            products,
            veilingKlok.HighestPrice,
            veilingKlok.LowestPrice
        );
        return VeilingKlokMapper.Minimal.ToOutputDto(veilingKlok, info);
    }
}
