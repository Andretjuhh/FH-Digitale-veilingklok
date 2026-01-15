using Application.DTOs.Output;
using Application.Repositories;
using MediatR;

namespace Application.UseCases.Product;

public sealed record GetLatestPricesQuery(int Limit = 10)
    : IRequest<List<PriceHistoryItemOutputDto>>;

public sealed class GetLatestPricesHandler
    : IRequestHandler<GetLatestPricesQuery, List<PriceHistoryItemOutputDto>>
{
    private readonly IProductRepository _productRepository;

    public GetLatestPricesHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<PriceHistoryItemOutputDto>> Handle(
        GetLatestPricesQuery request,
        CancellationToken cancellationToken
    )
    {
        var limit = request.Limit <= 0 ? 10 : request.Limit;
        var items = await _productRepository.GetLatestPricesAsync(limit);
        return items
            .Select(item => new PriceHistoryItemOutputDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                KwekerId = item.KwekerId,
                KwekerName = item.KwekerName,
                Price = item.Price,
                PurchasedAt = item.PurchasedAt
            })
            .ToList();
    }
}
