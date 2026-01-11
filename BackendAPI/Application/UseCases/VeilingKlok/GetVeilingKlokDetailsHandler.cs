using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.Common.Models;
using Application.DTOs.Output;
using Application.Repositories;
using MediatR;

namespace Application.UseCases.VeilingKlok;

public sealed record GetVeilingKlokDetailsCommand(Guid Id) : IRequest<VeilingKlokDetailsOutputDto>;

public sealed class GetVeilingDetailsKlokHandler
    : IRequestHandler<GetVeilingKlokDetailsCommand, VeilingKlokDetailsOutputDto>
{
    private readonly IVeilingKlokRepository _veilingKlokRepository;
    private readonly IProductRepository _productRepository;

    public GetVeilingDetailsKlokHandler(IVeilingKlokRepository veilingKlokRepository,
        IProductRepository productRepository)
    {
        _veilingKlokRepository = veilingKlokRepository;
        _productRepository = productRepository;
    }

    public async Task<VeilingKlokDetailsOutputDto> Handle(
        GetVeilingKlokDetailsCommand request,
        CancellationToken cancellationToken
    )
    {
        // Fetch veiling klok with bid count
        var result = await _veilingKlokRepository.GetByIdWithBidsCount(request.Id) ??
                     throw RepositoryException.NotFoundVeilingKlok();

        // Extract veiling klok and bid count
        var veilingKlok = result.VeilingKlok;
        var bidCount = result.BidCount;

        // Fetch associated products
        var products = (await _productRepository.GetAllByVeilingKlokIdWithKwekerInfoAsync(veilingKlok.Id))
            .Select(r => ProductMapper.ToOutputDto(r.Product, r.Kweker)).ToList();
        if (products.Count == 0)
        {
            products = (await _productRepository.GetAllByOrderItemsVeilingKlokIdWithKwekerInfoAsync(veilingKlok.Id))
                .Select(r => ProductMapper.ToOutputDto(r.Product, r.Kweker)).ToList();
        }


        // Create extra info and map to output DTO
        var extraInfo = new VeilingKlokExtraInfo<ProductDetailsOutputDto>(bidCount, products);
        return VeilingKlokMapper.ToOutputDto(veilingKlok, extraInfo);
    }
}
