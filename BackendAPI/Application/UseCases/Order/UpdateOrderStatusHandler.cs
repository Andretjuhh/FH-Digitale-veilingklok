using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Order;

public sealed record UpdateOrderStatusCommand(Guid OrderId, OrderStatus Status)
    : IRequest<OrderOutputDto>;

public sealed class UpdateOrderStatusHandler
    : IRequestHandler<UpdateOrderStatusCommand, OrderOutputDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;
    private readonly IVeilingKlokRepository _veilingKlokRepository;

    public UpdateOrderStatusHandler(
        IUnitOfWork unitOfWork,
        IOrderRepository orderRepository,
        IVeilingKlokRepository veilingKlokRepository
    )
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
        _veilingKlokRepository = veilingKlokRepository;
    }

    public async Task<OrderOutputDto> Handle(
        UpdateOrderStatusCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var order = await _orderRepository.GetByIdAsync(request.OrderId)
                        ?? throw RepositoryException.NotFoundOrder();

            // Retrieve associated VeilingKlok to validate status change
            var klok = await _veilingKlokRepository.GetByIdAsync(order.VeilingKlokId)
                       ?? throw RepositoryException.NotFoundOrder();

            // If Klok is still under auction is not possible to change status active
            if (klok.Status < VeilingKlokStatus.Ended)
                throw CustomException.InvalidOperationKlokStillRunning();

            // Business rules for status transition can be added here
            order.UpdateOrderStatus(request.Status);
            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return OrderMapper.ToOutputDto(order);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}