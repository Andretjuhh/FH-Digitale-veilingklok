using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Output;
using Application.Repositories;
using MediatR;

namespace Application.UseCases.VeilingKlok;

public sealed record GetVeilingKlokProductsQuery(Guid KlokId) : IRequest<List<ProductOutputDto>>;

public sealed class GetVeilingKlokProductsHandler
    : IRequestHandler<GetVeilingKlokProductsQuery, List<ProductOutputDto>>
{
    private readonly IVeilingKlokRepository _veilingKlokRepository;
    private readonly IProductRepository _productRepository;

    public GetVeilingKlokProductsHandler(
        IVeilingKlokRepository veilingKlokRepository,
        IProductRepository productRepository
    )
    {
        _veilingKlokRepository = veilingKlokRepository;
        _productRepository = productRepository;
    }

    public async Task<List<ProductOutputDto>> Handle(
        GetVeilingKlokProductsQuery request,
        CancellationToken cancellationToken
    )
    {
        // Verify VeilingKlok exists
        var veilingKlok = await _veilingKlokRepository.GetByIdAsync(request.KlokId)
                          ?? throw RepositoryException.NotFoundVeilingKlok();

        // Get products associated with the VeilingKlok (ordered by position)
        var productIds = veilingKlok.GetOrderedProductIds();

        if (productIds.Count == 0)
            return new List<ProductOutputDto>();

        // Get products with kweker info
        var productsWithKweker = await _productRepository.GetAllByIdsWithKwekerInfoAsync(productIds);

        // Map to output DTOs
        return productsWithKweker
            .Select(r => ProductMapper.Minimal.ToOutputDto(r.Product, r.Kweker))
            .ToList();
    }
}