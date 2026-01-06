using Application.Common.Mappers;
using Application.DTOs.Output;
using Application.Repositories;
using MediatR;

namespace Application.UseCases.Product;

public sealed record GetProductsQuery(
    string? NameFilter,
    decimal? MaxPrice,
    Guid? KwekerId,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedOutputDto<ProductOutputDto>>;

public sealed class GetProductsHandler
    : IRequestHandler<GetProductsQuery, PaginatedOutputDto<ProductOutputDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PaginatedOutputDto<ProductOutputDto>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken
    )
    {
        var (items, totalCount) = await _productRepository.GetAllWithFilterAsync(
            request.NameFilter,
            request.MaxPrice,
            request.KwekerId,
            request.PageNumber,
            request.PageSize
        );

        return new PaginatedOutputDto<ProductOutputDto>
        {
            Data = items
                .Select(item => ProductMapper.Minimal.ToOutputDto(item.Product, item.Kweker))
                .ToList(),
            TotalCount = totalCount,
            Page = request.PageNumber,
            Limit = request.PageSize,
        };
    }
}
