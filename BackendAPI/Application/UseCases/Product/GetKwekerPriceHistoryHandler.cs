using Application.DTOs.Output;
using Application.Repositories;
using MediatR;

namespace Application.UseCases.Product;

public sealed record GetKwekerPriceHistoryQuery(Guid KwekerId, int Limit = 10)
    : IRequest<List<PriceHistoryItemOutputDto>>;

public sealed class GetKwekerPriceHistoryHandler
    : IRequestHandler<GetKwekerPriceHistoryQuery, List<PriceHistoryItemOutputDto>>
{
    private readonly IProductRepository _productRepository;

    public GetKwekerPriceHistoryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<PriceHistoryItemOutputDto>> Handle(
        GetKwekerPriceHistoryQuery request,
        CancellationToken cancellationToken
    )
    {
        var limit = request.Limit <= 0 ? 10 : request.Limit;
        var items = await _productRepository.GetLatestPricesByKwekerAsync(request.KwekerId, limit);
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
