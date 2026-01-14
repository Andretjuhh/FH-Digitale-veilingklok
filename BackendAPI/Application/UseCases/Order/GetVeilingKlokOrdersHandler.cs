using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Output;
using Application.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Order;

public sealed record GetVeilingKlokOrdersQuery(
    Guid KlokId,
    OrderStatus? StatusFilter = null,
    DateTime? BeforeDate = null,
    DateTime? AfterDate = null
) : IRequest<List<OrderOutputDto>>;

public sealed class GetVeilingKlokOrdersHandler
    : IRequestHandler<GetVeilingKlokOrdersQuery, List<OrderOutputDto>>
{
    private readonly IVeilingKlokRepository _veilingKlokRepository;
    private readonly IOrderRepository _orderRepository;

    public GetVeilingKlokOrdersHandler(
        IVeilingKlokRepository veilingKlokRepository,
        IOrderRepository orderRepository
    )
    {
        _veilingKlokRepository = veilingKlokRepository;
        _orderRepository = orderRepository;
    }

    public async Task<List<OrderOutputDto>> Handle(
        GetVeilingKlokOrdersQuery request,
        CancellationToken cancellationToken
    )
    {
        // Verify VeilingKlok exists
        _ = await _veilingKlokRepository.GetByIdAsync(request.KlokId)
            ?? throw RepositoryException.NotFoundVeilingKlok();

        // Get all orders for this VeilingKlok with filters (no pagination)
        var (orders, _) = await _orderRepository.GetAllWithFilterAsync(
            request.StatusFilter,
            request.BeforeDate,
            request.AfterDate,
            null, // productId
            null, // koperId - get all orders for any koper
            request.KlokId, // klokId
            1, // pageNumber
            int.MaxValue // pageSize - get all records
        );

        return orders.Select(OrderMapper.ToOutputDto).ToList();
    }
}