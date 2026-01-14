using Application.Common.Mappers;
using Application.Common.Models;
using Application.DTOs.Output;
using Application.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.VeilingKlok;

public sealed record GetVeilingKlokkenQuery(
    VeilingKlokStatus? StatusFilter,
    string? Region,
    DateTime? ScheduledAfter,
    DateTime? ScheduledBefore,
    DateTime? StartedAfter,
    DateTime? StartedBefore,
    DateTime? EndedAfter,
    DateTime? EndedBefore,
    Guid? MeesterId,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedOutputDto<VeilingKlokOutputDto>>;

public sealed class GetVeilingKlokkenHandler
    : IRequestHandler<GetVeilingKlokkenQuery, PaginatedOutputDto<VeilingKlokOutputDto>>
{
    private readonly IVeilingKlokRepository _veilingKlokRepository;
    private readonly IProductRepository _productRepository;

    public GetVeilingKlokkenHandler(
        IVeilingKlokRepository veilingKlokRepository,
        IProductRepository productRepository
    )
    {
        _veilingKlokRepository = veilingKlokRepository;
        _productRepository = productRepository;
    }

    public async Task<PaginatedOutputDto<VeilingKlokOutputDto>> Handle(
        GetVeilingKlokkenQuery request,
        CancellationToken cancellationToken
    )
    {
        var (items, totalCount) = await _veilingKlokRepository.GetAllWithFilterAndBidsAsync(
            request.StatusFilter,
            request.Region,
            request.ScheduledAfter,
            request.ScheduledBefore,
            request.StartedAfter,
            request.StartedBefore,
            request.EndedAfter,
            request.EndedBefore,
            request.MeesterId,
            request.PageNumber,
            request.PageSize
        );

        var results = new List<VeilingKlokOutputDto>();

        foreach (var item in items)
        {
            var veilingKlok = item.VeilingKlok;
            var bidCount = item.BidCount;

            var productIds = veilingKlok.GetOrderedProductIds();

            var products = productIds.Count == 0
                ? new List<ProductOutputDto>()
                : (await _productRepository.GetAllByIdsWithKwekerInfoAsync(productIds))
                .Select(r => ProductMapper.Minimal.ToOutputDto(r.Product, r.Kweker))
                .ToList();

            var info = new VeilingKlokExtraInfo<ProductOutputDto>(
                bidCount,
                products,
                veilingKlok.HighestPrice,
                veilingKlok.LowestPrice
            );

            results.Add(VeilingKlokMapper.Minimal.ToOutputDto(veilingKlok, info));
        }

        return new PaginatedOutputDto<VeilingKlokOutputDto>
        {
            Data = results,
            TotalCount = totalCount,
            Page = request.PageNumber,
            Limit = request.PageSize
        };
    }
}