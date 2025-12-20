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
) : IRequest<(IEnumerable<OrderOutputDto> data, int totalCount)>;

public sealed class GetProductOrdersHandler
    : IRequestHandler<GetProductOrdersCommand, (IEnumerable<OrderOutputDto> data, int totalCount)>
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

    public async Task<(IEnumerable<OrderOutputDto> data, int totalCount)> Handle(
        GetProductOrdersCommand request,
        CancellationToken cancellationToken
    )
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, request.KwekerId) ??
                      throw RepositoryException.NotFoundProduct();
        var (orders, totalCount) = await _orderRepository.GetAllWithFilterAsync(
            request.StatusFilter,
            request.BeforeDate,
            request.AfterDate,
            product.Id,
            null,
            null,
            request.PageNumber,
            request.PageSize
        );
        return (orders.Select(OrderMapper.ToOutputDto), totalCount);
    }
}