using Application.Common.Exceptions;
using Application.Repositories;
using Application.Services;
using MediatR;

namespace Application.UseCases.Order;

public sealed record DeleteOrderCommand(Guid OrderId) : IRequest;

public sealed class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;

    public DeleteOrderHandler(IUnitOfWork unitOfWork, IOrderRepository orderRepository)
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
    }

    public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var order =
                await _orderRepository.GetByIdAsync(request.OrderId)
                ?? throw RepositoryException.NotFoundOrder();

            // If order have products, cannot delete
            if (order.ProductsIds.Count > 0)
                throw CustomException.InvalidOperation();

            await _orderRepository.DeleteAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}