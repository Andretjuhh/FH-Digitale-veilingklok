using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Output;
using Application.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Product;

public sealed record GetProductOrdersCommand(
    Guid ProductId,
    Guid KwekerId,
    OrderStatus? StatusFilter = null,
    DateTime? BeforeDate = null,
    DateTime? AfterDate = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedOutputDto<OrderOutputDto>>;

public sealed class GetProductOrdersHandler
    : IRequestHandler<GetProductOrdersCommand, PaginatedOutputDto<OrderOutputDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;

    public GetProductOrdersHandler(
        IProductRepository productRepository,
        IOrderRepository orderRepository
    )
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
    }

    public async Task<PaginatedOutputDto<OrderOutputDto>> Handle(
        GetProductOrdersCommand request,
        CancellationToken cancellationToken
    )
    {
        var product =
            await _productRepository.GetByIdAsync(request.ProductId, request.KwekerId)
            ?? throw RepositoryException.NotFoundProduct();
        var (items, totalCount) = await _orderRepository.GetAllWithFilterAsync(
            request.StatusFilter,
            request.BeforeDate,
            request.AfterDate,
            product.Id,
            null,
            null,
            request.PageNumber,
            request.PageSize
        );

        return new PaginatedOutputDto<OrderOutputDto>
        {
            Data = items.Select(x => OrderMapper.ToOutputDto(x.order, x.products)).ToList(),
            TotalCount = totalCount,
            Page = request.PageNumber,
            Limit = request.PageSize,
        };
    }
}
