using Application.Common.Exceptions;
using Application.Common.Mappers;
using Application.DTOs.Output;
using Application.Repositories;
using Application.Services;
using Domain.Enums;
using MediatR;

namespace Application.UseCases.Order;

public sealed record UpdateOrderProductCommand(Guid OrderId, Guid ProductItemId, int Quantity)
    : IRequest<OrderOutputDto>;

public sealed class UpdateOrderProductHandler
    : IRequestHandler<UpdateOrderProductCommand, OrderOutputDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IVeilingKlokRepository _veilingKlokRepository;

    public UpdateOrderProductHandler(
        IUnitOfWork unitOfWork,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IVeilingKlokRepository veilingKlokRepository
    )
    {
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _veilingKlokRepository = veilingKlokRepository;
    }

    public async Task<OrderOutputDto> Handle(
        UpdateOrderProductCommand request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            var order =
                await _orderRepository.GetByIdAsync(request.OrderId)
                ?? throw RepositoryException.NotFoundOrder();

            // Retrieve the status of the associated VeilingKlok
            var veilingKlokStatus = await _veilingKlokRepository.GetStatusAsync(order.VeilingKlokId, cancellationToken);

            // If a clock is running you cannot change the quantity of an order item
            if (veilingKlokStatus < VeilingKlokStatus.Ended)
                throw CustomException.InvalidOperation();

            // Find the order item to update
            var orderItem =
                order.OrderItems.FirstOrDefault(oi => oi.ProductId == request.ProductItemId)
                ?? throw RepositoryException.NotFoundProduct();

            var quantityDiff = request.Quantity - orderItem.Quantity;

            // If quantity is changed to an higher number it should fail, after an veiling
            // you cannot increase the quantity of an order item.
            if (quantityDiff > 0)
                throw CustomException.InvalidOperation();

            var product =
                await _productRepository.GetByIdAsync(orderItem.ProductId)
                ?? throw RepositoryException.NotFoundProduct();

            product.IncreaseStock(-quantityDiff);
            orderItem.UpdateQuantity(request.Quantity);

            _orderRepository.Update(order);
            _productRepository.Update(product);
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