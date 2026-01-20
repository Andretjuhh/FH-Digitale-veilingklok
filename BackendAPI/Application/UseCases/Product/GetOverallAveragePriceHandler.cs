using Application.DTOs.Output;
using Application.Repositories;
using MediatR;

namespace Application.UseCases.Product;

public sealed record GetOverallAveragePriceQuery() : IRequest<OverallAveragePriceOutputDto>;

public sealed class GetOverallAveragePriceHandler
    : IRequestHandler<GetOverallAveragePriceQuery, OverallAveragePriceOutputDto>
{
    private readonly IProductRepository _productRepository;

    public GetOverallAveragePriceHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<OverallAveragePriceOutputDto> Handle(
        GetOverallAveragePriceQuery request,
        CancellationToken cancellationToken
    )
    {
        var (avg, count) = await _productRepository.GetAveragePriceAllAsync();
        return new OverallAveragePriceOutputDto
        {
            AveragePrice = avg,
            SampleCount = count
        };
    }
}
