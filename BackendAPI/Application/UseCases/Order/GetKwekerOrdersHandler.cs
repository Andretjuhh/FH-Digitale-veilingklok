using Application.Common.Mappers;
using Application.DTOs.Output;
using Application.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Order;

public sealed record GetKwekerOrdersCommand(
    Guid KwekerId,
    string? ProductNameFilter,
    string? KoperNameFilter,
    OrderStatus? StatusFilter = null,
    DateTime? BeforeDate = null,
    DateTime? AfterDate = null,
    Guid? ProductId = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedOutputDto<OrderKwekerOutputDto>>;

public class GetKwekerOrdersHandler
    : IRequestHandler<GetKwekerOrdersCommand, PaginatedOutputDto<OrderKwekerOutputDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetKwekerOrdersHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PaginatedOutputDto<OrderKwekerOutputDto>> Handle(
        GetKwekerOrdersCommand request,
        CancellationToken cancellationToken
    )
    {
        // Use the optimized repository method with filters
        var (orderData, totalCount) = await _orderRepository.GetAllKwekerWithFilterAsync(
            request.ProductNameFilter,
            request.KoperNameFilter,
            request.StatusFilter,
            request.BeforeDate,
            request.AfterDate,
            request.ProductId,
            request.KwekerId,
            request.PageNumber,
            request.PageSize
        );

        // Map the results using the optimized mapper
        var result = orderData.Select(x => OrderMapper.KwekerOrders.ToOutputDto(x)).ToList();

        return new PaginatedOutputDto<OrderKwekerOutputDto>
        {
            Page = request.PageNumber,
            Limit = request.PageSize,
            TotalCount = totalCount,
            Data = result
        };
    }
}