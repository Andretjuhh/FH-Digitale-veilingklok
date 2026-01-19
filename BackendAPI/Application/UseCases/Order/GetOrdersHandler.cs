using Application.Common.Mappers;
using Application.DTOs.Output;
using Application.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Order;

public sealed record GetOrdersCommand(
    Guid KoperId,
    OrderStatus? Status,
    DateTime? BeforeDate,
    DateTime? AfterDate,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedOutputDto<OrderOutputDto>>;

public sealed class GetOrdersHandler
    : IRequestHandler<GetOrdersCommand, PaginatedOutputDto<OrderOutputDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PaginatedOutputDto<OrderOutputDto>> Handle(
        GetOrdersCommand request,
        CancellationToken c
    )
    {
        var (items, totalCount) = await _orderRepository.GetAllWithFilterAsync(
            request.Status,
            request.BeforeDate,
            request.AfterDate,
            null,
            request.KoperId,
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
