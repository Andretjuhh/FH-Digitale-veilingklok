using Application.Common.Exceptions;
using Application.DTOs.Output;
using Application.Repositories;
using MediatR;

namespace Application.UseCases.Product;

public sealed record GetKwekerAveragePriceQuery(Guid KwekerId, int? Limit = null)
    : IRequest<KwekerAveragePriceOutputDto>;

public sealed class GetKwekerAveragePriceHandler
    : IRequestHandler<GetKwekerAveragePriceQuery, KwekerAveragePriceOutputDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IKwekerRepository _kwekerRepository;

    public GetKwekerAveragePriceHandler(
        IProductRepository productRepository,
        IKwekerRepository kwekerRepository
    )
    {
        _productRepository = productRepository;
        _kwekerRepository = kwekerRepository;
    }

    public async Task<KwekerAveragePriceOutputDto> Handle(
        GetKwekerAveragePriceQuery request,
        CancellationToken cancellationToken
    )
    {
        var avg = await _productRepository.GetAveragePriceByKwekerAsync(
            request.KwekerId,
            request.Limit
        );

        if (avg != null)
        {
            return new KwekerAveragePriceOutputDto
            {
                KwekerId = avg.KwekerId,
                KwekerName = avg.KwekerName,
                AveragePrice = avg.AveragePrice,
                SampleCount = avg.SampleCount
            };
        }

        var kweker = await _kwekerRepository.GetKwekerByIdAsync(request.KwekerId);
        if (kweker == null)
            throw RepositoryException.NotFoundAccount();

        return new KwekerAveragePriceOutputDto
        {
            KwekerId = kweker.Id,
            KwekerName = kweker.CompanyName,
            AveragePrice = 0,
            SampleCount = 0
        };
    }
}
