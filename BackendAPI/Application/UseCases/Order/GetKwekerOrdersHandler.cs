using Application.DTOs.Output;
using Application.Repositories;
using Application.Common.Mappers;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Order;

public sealed record GetKwekerOrdersCommand(
    Guid KwekerId,
    OrderStatus? StatusFilter = null,
    DateTime? BeforeDate = null,
    DateTime? AfterDate = null,
    Guid? ProductId = null,
    int PageNumber = 1,
    int PageSize = 10)
    : IRequest<PaginatedOutputDto<OrderKwekerOutput>>;

public class GetKwekerOrdersHandler : IRequestHandler<GetKwekerOrdersCommand, PaginatedOutputDto<OrderKwekerOutput>>
{
    private readonly IOrderRepository _orderRepository;

    public GetKwekerOrdersHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PaginatedOutputDto<OrderKwekerOutput>> Handle(
        GetKwekerOrdersCommand request,
        CancellationToken cancellationToken)
    {
        // Use the optimized repository method with filters
        var (orderData, totalCount) = await _orderRepository.GetAllKwekerWithFilterAsync(
            request.StatusFilter,
            request.BeforeDate,
            request.AfterDate,
            request.ProductId,
            request.KwekerId,
            request.PageNumber,
            request.PageSize);

        // Map the results using the optimized mapper
        var result = orderData
            .Select(OrderMapper.KwekerOrders.ToOutputDto)
            .ToList();

        return new PaginatedOutputDto<OrderKwekerOutput>
        {
            Page = request.PageNumber,
            Limit = request.PageSize,
            TotalCount = totalCount,
            Data = result
        };
    }
}